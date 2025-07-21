@echo off
echo =====================================================
echo GestionTicketClinisys Database Setup
echo =====================================================
echo.

echo Choose an option:
echo 1. Full setup (migrations + data inserts)
echo 2. Only apply migrations
echo 3. Only insert data
echo 4. Clear data and insert fresh data
echo 5. Verify database status only
echo 6. Exit
echo.

set /p choice="Enter your choice (1-6): "

if "%choice%"=="1" (
    echo Running full setup...
    powershell -ExecutionPolicy Bypass -File "Execute_Database_Setup.ps1"
) else if "%choice%"=="2" (
    echo Applying migrations only...
    dotnet ef database update
) else if "%choice%"=="3" (
    echo Inserting data only...
    powershell -ExecutionPolicy Bypass -File "Execute_Database_Setup.ps1" -SkipMigrations
) else if "%choice%"=="4" (
    echo Clearing and inserting fresh data...
    powershell -ExecutionPolicy Bypass -File "Execute_Database_Setup.ps1" -ClearData
) else if "%choice%"=="5" (
    echo Verifying database status...
    powershell -ExecutionPolicy Bypass -File "Execute_Database_Setup.ps1" -VerifyOnly
) else if "%choice%"=="6" (
    echo Exiting...
    exit /b 0
) else (
    echo Invalid choice. Please run the script again.
    pause
    exit /b 1
)

echo.
echo Press any key to continue...
pause >nul
