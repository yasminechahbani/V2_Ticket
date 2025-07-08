# GestionTicketClinisys

A Clinical Ticket Management System built with ASP.NET Core for managing support tickets and requests in healthcare environments.

## 🏥 Overview

**GestionTicketClinisys** is a web-based ticket management system specifically designed for clinical/healthcare environments. It provides a comprehensive solution for managing support requests, tracking issues, and organizing tasks across different teams and modules.

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0 (MVC)
- **Database**: SQL Server with Entity Framework Core 9.0.6
- **UI**: Razor Views with custom CSS and JavaScript
- **Architecture**: MVC pattern with Entity Framework Repository

## 📋 Features

### Core Functionality
- ✅ **Ticket Management**: Create, view, edit, and delete support tickets
- ✅ **User Management**: Manage users with roles and team assignments
- ✅ **Client Management**: Track clients who submit tickets
- ✅ **Team Organization**: Group users into teams for ticket assignment
- ✅ **Module Categorization**: Organize tickets by system modules
- ✅ **Task Breakdown**: Split tickets into smaller manageable tasks
- ✅ **Status Tracking**: Full lifecycle management from creation to closure
- ✅ **Priority Management**: Four-level priority system

### User Interface
- 🎯 **Dashboard Menu**: Card-based navigation system
- 📊 **Ticket Views**: Multiple views for different ticket management needs
- 🔍 **CRUD Operations**: Complete Create, Read, Update, Delete functionality
- 📱 **Responsive Design**: Works across different screen sizes

## 🗂️ Data Model

### Core Entities

#### Ticket (Central Entity)
```csharp
- Id, Title, Description, CreationDate
- Status: Open → InProgress → Resolved → Closed
- Priority: Low → Medium → High → Critical
- Relationships: Client, User, Team, Module, TaskItems
```

#### User
```csharp
- Id, UserName, Email, Role, Password
- Team assignment capability
```

#### Client
```csharp
- Id, Name, Email
- Represents ticket creators/customers
```

#### Team
```csharp
- Id, Name
- Groups users for ticket assignment
```

#### Module
```csharp
- Id, Name
- Categorizes tickets by system component
```

#### TaskItem
```csharp
- Id, Title, Description, Status, CreatedAt
- Breaks down tickets into subtasks
- Assigned to specific users
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd GestionTicketClinisys
   ```

2. **Configure Database**
   - Update connection string in `appsettings.json` if needed:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=CliniSysDb;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`

## 📁 Project Structure

```
GestionTicketClinisys/
├── Controllers/           # MVC Controllers
│   ├── TicketsController.cs
│   ├── UsersController.cs
│   ├── ClientsController.cs
│   ├── TeamsController.cs
│   ├── ModulesController.cs
│   └── HomeController.cs
├── Models/               # Data Models
│   ├── Ticket.cs
│   ├── User.cs
│   ├── Client.cs
│   ├── Team.cs
│   ├── Module.cs
│   ├── TaskItem.cs
│   ├── ApplicationDbContext.cs
│   └── Enums/
│       ├── TicketStatus.cs
│       ├── TicketPriority.cs
│       └── State.cs
├── Views/                # Razor Views
│   ├── Home/
│   ├── Tickets/
│   ├── Users/
│   ├── Clients/
│   ├── Teams/
│   ├── Modules/
│   └── Shared/
├── Migrations/           # EF Core Migrations
├── wwwroot/             # Static files (CSS, JS, images)
└── Program.cs           # Application entry point
```

## 🎯 Usage

### Main Navigation
The application provides a card-based menu system with the following options:

1. **📝 Nouvelle Demande** - Create new tickets
2. **⚙️ Gestion des demandes** - Manage existing tickets
3. **📋 Liste des fiches** - View all tickets
4. **📁 Demandes classées** - View archived/classified tickets
5. **📊 Mes tâches** - View assigned tasks
6. **👁️ Guide utilisateur** - User documentation
7. **👥 Changement du mot de passe** - Password management

### Ticket Workflow
1. **Creation**: Clients create tickets with title, description, and priority
2. **Assignment**: Tickets can be assigned to users or teams
3. **Categorization**: Associate tickets with specific modules
4. **Task Breakdown**: Split complex tickets into smaller tasks
5. **Status Updates**: Track progress through the workflow
6. **Resolution**: Mark tickets as resolved and eventually closed

## 🔧 Configuration

### Database Configuration
The application uses Entity Framework Core with SQL Server. Connection strings are configured in `appsettings.json`.

### Adding New Migrations
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## 🚧 Known Limitations & Future Enhancements

### Security Considerations
- ⚠️ **Authentication**: No authentication middleware currently implemented
- ⚠️ **Password Security**: Passwords stored in plain text (needs hashing)
- ⚠️ **Authorization**: No role-based access control

### Potential Improvements
- 🔐 Implement proper authentication and authorization
- 🔒 Add password hashing and security measures
- 📧 Email notifications for ticket updates
- 📊 Reporting and analytics dashboard
- 🔍 Advanced search and filtering
- 📱 Mobile app or PWA version
- 🌐 Multi-language support
- 📎 File attachment support
- 🕒 Time tracking functionality

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions, please contact the development team or create an issue in the repository.

---

**Note**: This is a clinical management system. Ensure compliance with healthcare data protection regulations (HIPAA, GDPR, etc.) before deploying in production environments.
