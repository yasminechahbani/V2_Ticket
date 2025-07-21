# =====================================================
# GestionTicketClinisys Database Setup Script
# =====================================================
# This PowerShell script will:
# 1. Check database connection
# 2. Apply EF Core migrations
# 3. Execute insert statements
# 4. Verify data insertion

param(
    [switch]$SkipMigrations,
    [switch]$ClearData,
    [switch]$VerifyOnly
)

# Configuration
$ProjectPath = "."
$ConnectionString = "Server=DESKTOP-KED8JSF\SQL2019;Database=CliniSysDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
$SqlFile = "Database_Inserts.sql"

Write-Host "=== GestionTicketClinisys Database Setup ===" -ForegroundColor Green
Write-Host "Project Path: $ProjectPath" -ForegroundColor Yellow
Write-Host "Database: CliniSysDb" -ForegroundColor Yellow
Write-Host ""

# Function to execute SQL command
function Execute-SqlCommand {
    param(
        [string]$Query,
        [string]$Description
    )
    
    try {
        Write-Host "Executing: $Description..." -ForegroundColor Cyan
        
        # Use sqlcmd if available, otherwise use .NET SqlConnection
        if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
            $result = sqlcmd -S "DESKTOP-KED8JSF\SQL2019" -d "CliniSysDb" -E -Q $Query
            Write-Host "✓ $Description completed" -ForegroundColor Green
            return $result
        } else {
            Write-Host "sqlcmd not found. Using .NET SqlConnection..." -ForegroundColor Yellow
            
            Add-Type -AssemblyName System.Data
            $connection = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
            $command = New-Object System.Data.SqlClient.SqlCommand($Query, $connection)
            
            $connection.Open()
            $result = $command.ExecuteScalar()
            $connection.Close()
            
            Write-Host "✓ $Description completed" -ForegroundColor Green
            return $result
        }
    }
    catch {
        Write-Host "✗ Error in $Description`: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Function to check database connection
function Test-DatabaseConnection {
    Write-Host "Testing database connection..." -ForegroundColor Cyan
    
    $testQuery = "SELECT 1"
    $result = Execute-SqlCommand -Query $testQuery -Description "Database connection test"
    
    if ($result) {
        Write-Host "✓ Database connection successful" -ForegroundColor Green
        return $true
    } else {
        Write-Host "✗ Database connection failed" -ForegroundColor Red
        return $false
    }
}

# Function to check if tables exist
function Test-TablesExist {
    Write-Host "Checking if tables exist..." -ForegroundColor Cyan
    
    $tableCheckQuery = @"
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Teams', 'Modules', 'Clients', 'Users', 'Tickets', 'TaskItems')
"@
    
    $tableCount = Execute-SqlCommand -Query $tableCheckQuery -Description "Table existence check"
    
    if ($tableCount -eq 6) {
        Write-Host "✓ All required tables exist" -ForegroundColor Green
        return $true
    } else {
        Write-Host "✗ Missing tables. Found $tableCount out of 6 required tables" -ForegroundColor Red
        return $false
    }
}

# Function to get current data counts
function Get-DataCounts {
    Write-Host "Getting current data counts..." -ForegroundColor Cyan
    
    $tables = @('Teams', 'Modules', 'Clients', 'Users', 'Tickets', 'TaskItems')
    $counts = @{}
    
    foreach ($table in $tables) {
        $query = "SELECT COUNT(*) FROM $table"
        $count = Execute-SqlCommand -Query $query -Description "Count $table"
        $counts[$table] = $count
        Write-Host "  $table`: $count records" -ForegroundColor White
    }
    
    return $counts
}

# Function to apply EF Core migrations
function Apply-Migrations {
    Write-Host "Applying EF Core migrations..." -ForegroundColor Cyan
    
    try {
        Push-Location $ProjectPath
        
        # Check if dotnet ef is available
        $efCheck = dotnet ef --version 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Installing dotnet ef tool..." -ForegroundColor Yellow
            dotnet tool install --global dotnet-ef
        }
        
        # Apply migrations
        Write-Host "Running: dotnet ef database update" -ForegroundColor Yellow
        dotnet ef database update
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Migrations applied successfully" -ForegroundColor Green
            return $true
        } else {
            Write-Host "✗ Migration failed" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "✗ Error applying migrations: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
    finally {
        Pop-Location
    }
}

# Function to execute SQL file
function Execute-SqlFile {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        Write-Host "✗ SQL file not found: $FilePath" -ForegroundColor Red
        return $false
    }
    
    Write-Host "Executing SQL file: $FilePath..." -ForegroundColor Cyan
    
    try {
        if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
            sqlcmd -S "DESKTOP-KED8JSF\SQL2019" -d "CliniSysDb" -E -i $FilePath
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ SQL file executed successfully" -ForegroundColor Green
                return $true
            } else {
                Write-Host "✗ SQL file execution failed" -ForegroundColor Red
                return $false
            }
        } else {
            Write-Host "sqlcmd not available. Please install SQL Server Command Line Utilities." -ForegroundColor Red
            Write-Host "Alternative: Execute the SQL file manually in SQL Server Management Studio." -ForegroundColor Yellow
            return $false
        }
    }
    catch {
        Write-Host "✗ Error executing SQL file: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Main execution
try {
    # Test database connection
    if (-not (Test-DatabaseConnection)) {
        Write-Host "Cannot proceed without database connection. Please check:" -ForegroundColor Red
        Write-Host "1. SQL Server is running" -ForegroundColor Yellow
        Write-Host "2. Connection string is correct" -ForegroundColor Yellow
        Write-Host "3. Database CliniSysDb exists" -ForegroundColor Yellow
        exit 1
    }
    
    # Apply migrations if not skipped
    if (-not $SkipMigrations) {
        if (-not (Apply-Migrations)) {
            Write-Host "Migration failed. Continuing with table check..." -ForegroundColor Yellow
        }
    }
    
    # Check if tables exist
    if (-not (Test-TablesExist)) {
        Write-Host "Tables are missing. Please run migrations first:" -ForegroundColor Red
        Write-Host "dotnet ef database update" -ForegroundColor Yellow
        exit 1
    }
    
    # Get current data counts
    Write-Host ""
    Write-Host "=== Current Data Status ===" -ForegroundColor Green
    $currentCounts = Get-DataCounts
    
    # If verify only, exit here
    if ($VerifyOnly) {
        Write-Host ""
        Write-Host "Verification complete." -ForegroundColor Green
        exit 0
    }
    
    # Clear data if requested
    if ($ClearData) {
        Write-Host ""
        Write-Host "Clearing existing data..." -ForegroundColor Yellow
        $clearQuery = @"
DELETE FROM TaskItems;
DELETE FROM Tickets;
DELETE FROM Users;
DELETE FROM Clients;
DELETE FROM Modules;
DELETE FROM Teams;
"@
        Execute-SqlCommand -Query $clearQuery -Description "Clear existing data"
    }
    
    # Execute insert statements
    Write-Host ""
    Write-Host "=== Executing Data Inserts ===" -ForegroundColor Green
    
    if (Execute-SqlFile -FilePath $SqlFile) {
        Write-Host ""
        Write-Host "=== Final Data Status ===" -ForegroundColor Green
        Get-DataCounts
        
        Write-Host ""
        Write-Host "=== Setup Complete! ===" -ForegroundColor Green
        Write-Host "Database is ready for use." -ForegroundColor White
    } else {
        Write-Host "Data insertion failed." -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "Script execution failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Script completed. You can now start the application:" -ForegroundColor Cyan
Write-Host "dotnet run" -ForegroundColor Yellow
