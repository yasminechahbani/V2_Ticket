# 🎫 Gestion Ticket Clinisys

Un système de gestion de tickets moderne et complet développé avec ASP.NET Core MVC pour la gestion des demandes cliniques.

## 📋 Table des Matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Technologies Utilisées](#technologies-utilisées)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Configuration](#configuration)
- [Utilisation](#utilisation)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
- [Base de Données](#base-de-données)
- [Fonctionnalités Avancées](#fonctionnalités-avancées)
- [Dépannage](#dépannage)
- [Contribution](#contribution)

## 🎯 Aperçu

Le système Gestion Ticket Clinisys est une application web complète conçue pour gérer efficacement les tickets de demandes dans un environnement clinique. Il offre une interface utilisateur moderne avec des fonctionnalités avancées de gestion, de filtrage et d'export.

### Captures d'écran

- Interface principale avec tableau de bord
- Modales de création/édition avec éditeur de texte riche
- Système de filtrage et recherche en temps réel
- Export CSV et suppression en lot

## ✨ Fonctionnalités

### 🎪 Gestion des Tickets

- ✅ **Création de tickets** avec modal dialog riche
- ✅ **Édition en place** avec pré-remplissage des données
- ✅ **Suppression sécurisée** avec confirmation
- ✅ **Suppression en lot** pour plusieurs tickets
- ✅ **Export CSV** des tickets sélectionnés

### 🔍 Recherche et Filtrage

- ✅ **Recherche en temps réel** multi-champs
- ✅ **Filtres par statut** (Open, InProgress, Resolved, Closed)
- ✅ **Filtres par type** (Maintenance, Développement, Support)
- ✅ **Filtres par priorité** (Low, Medium, High, Critical)
- ✅ **Filtrage par plage de dates**
- ✅ **Bouton de réinitialisation** des filtres

### 📝 Éditeur de Contenu

- ✅ **Éditeur de texte riche** avec formatage
- ✅ **Support des liens** et listes
- ✅ **Upload de fichiers** (préparé)
- ✅ **Commentaires formatés**

### 👥 Gestion des Entités

- ✅ **Clients** - Gestion des demandeurs
- ✅ **Utilisateurs** - Assignation des tickets
- ✅ **Équipes** - Organisation par équipes
- ✅ **Modules** - Catégorisation par modules

### 🎨 Interface Utilisateur

- ✅ **Design responsive** et moderne
- ✅ **Modales interactives** pour toutes les actions
- ✅ **Badges colorés** pour statuts et priorités
- ✅ **Sélection multiple** avec checkboxes
- ✅ **Compteurs dynamiques** de sélection

## 🛠️ Technologies Utilisées

### Backend

- **ASP.NET Core 8.0** - Framework web principal
- **Entity Framework Core 9.0.6** - ORM pour la base de données
- **SQL Server** - Base de données relationnelle
- **C# 12** - Langage de programmation

### Frontend

- **HTML5/CSS3** - Structure et style
- **JavaScript ES6+** - Interactivité côté client
- **Bootstrap** - Framework CSS (intégré)
- **Fetch API** - Communication AJAX

### Outils de Développement

- **Visual Studio 2022** - IDE recommandé
- **SQL Server Management Studio** - Gestion de base de données
- **Entity Framework Tools** - Migrations et scaffolding

## 📋 Prérequis

### Logiciels Requis

- **.NET 8.0 SDK** ou supérieur
- **SQL Server 2019** ou supérieur (ou SQL Server Express)
- **Visual Studio 2022** ou **VS Code** avec extensions C#
- **Git** pour le contrôle de version

### Connaissances Recommandées

- C# et ASP.NET Core MVC
- Entity Framework Core
- HTML/CSS/JavaScript
- SQL Server

## 🚀 Installation

### 1. Cloner le Projet

```bash
git clone [URL_DU_REPOSITORY]
cd GestionTicketClinisys
```

### 2. Restaurer les Packages

```bash
dotnet restore
```

### 3. Configuration de la Base de Données

Modifier la chaîne de connexion dans `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CliniSysDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. Appliquer les Migrations

```bash
dotnet ef database update
```

### 5. Lancer l'Application

```bash
dotnet run
```

L'application sera accessible sur `http://localhost:5296`

## ⚙️ Configuration

### Chaîne de Connexion

Configurez votre chaîne de connexion SQL Server dans `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=VOTRE_SERVEUR;Database=CliniSysDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Variables d'Environnement

Pour la production, utilisez des variables d'environnement:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=VOTRE_CHAINE_DE_CONNEXION`

## 📖 Utilisation

### Accès à l'Application

1. Démarrez l'application avec `dotnet run`
2. Ouvrez votre navigateur sur `http://localhost:5296`
3. Naviguez vers "Gestion des demandes"

### Création d'un Ticket

1. Cliquez sur le bouton **"+ Demande"**
2. Remplissez le formulaire modal:
   - **Demandeur** (obligatoire)
   - **Titre** (obligatoire)
   - **Description** (obligatoire)
   - **Statut, Priorité, Type** (optionnels)
   - **Utilisateur, Équipe, Module** (optionnels)
   - **Commentaire** avec éditeur riche
3. Cliquez **"Créer"**

### Modification d'un Ticket

1. Cliquez sur l'icône **✏️** dans la ligne du ticket
2. Modifiez les champs dans le modal pré-rempli
3. Cliquez **"Mettre à jour"**

### Suppression de Tickets

- **Suppression individuelle**: Cliquez sur **🗑️** et confirmez
- **Suppression en lot**:
  1. Sélectionnez les tickets avec les checkboxes
  2. Cliquez **"🗑️ Supprimer (X)"**
  3. Confirmez la suppression

### Export de Données

1. Sélectionnez les tickets à exporter
2. Cliquez **"📤 Export CSV (X)"**
3. Le fichier CSV sera téléchargé automatiquement

### Recherche et Filtrage

- **Recherche**: Tapez dans la barre de recherche
- **Filtres**: Utilisez les dropdowns pour filtrer par statut, type, priorité
- **Dates**: Sélectionnez une plage de dates
- **Réinitialisation**: Cliquez **"🔄 Effacer"**

## 🏗️ Architecture

### Structure du Projet

```
GestionTicketClinisys/
├── Controllers/           # Contrôleurs MVC
│   ├── TicketsController.cs    # Gestion des tickets
│   ├── ClientsController.cs    # Gestion des clients
│   ├── UsersController.cs      # Gestion des utilisateurs
│   ├── TeamsController.cs      # Gestion des équipes
│   └── ModulesController.cs    # Gestion des modules
├── Models/               # Modèles de données
│   ├── Ticket.cs              # Modèle principal
│   ├── Client.cs              # Modèle client
│   ├── User.cs                # Modèle utilisateur
│   ├── Team.cs                # Modèle équipe
│   ├── Module.cs              # Modèle module
│   ├── Enums/                 # Énumérations
│   └── ApplicationDbContext.cs # Contexte EF
├── Views/                # Vues Razor
│   ├── Tickets/               # Vues des tickets
│   │   └── GestionDemandes.cshtml # Vue principale
│   ├── Shared/                # Vues partagées
│   └── _Layout.cshtml         # Layout principal
├── wwwroot/              # Ressources statiques
│   ├── css/                   # Feuilles de style
│   ├── js/                    # Scripts JavaScript
│   └── lib/                   # Bibliothèques
└── Migrations/           # Migrations EF Core
```

### Modèle de Données

#### Entité Ticket (Principale)

```csharp
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }           // Titre du ticket
    public string Description { get; set; }     // Description
    public DateTime CreationDate { get; set; }  // Date de création
    public TicketStatus Status { get; set; }    // Statut
    public TicketPriority Priority { get; set; } // Priorité
    public TicketType? Type { get; set; }       // Type (optionnel)

    // Relations
    public int ClientId { get; set; }           // Client (obligatoire)
    public Client Client { get; set; }

    public int? UserId { get; set; }            // Utilisateur assigné
    public User? User { get; set; }

    public int? TeamId { get; set; }            // Équipe assignée
    public Team? Team { get; set; }

    public int? ModuleId { get; set; }          // Module
    public Module? Module { get; set; }

    // Champs additionnels
    public int? DurationDays { get; set; }      // Durée en jours
    public int? DurationMonths { get; set; }    // Durée en mois
    public string? RichTextComment { get; set; } // Commentaire riche
    public string? AttachedFiles { get; set; }  // Fichiers joints
}
```

#### Énumérations

```csharp
public enum TicketStatus
{
    Open,        // Ouvert
    InProgress,  // En cours
    Resolved,    // Résolu
    Closed       // Fermé
}

public enum TicketPriority
{
    Low,         // Basse
    Medium,      // Moyenne
    High,        // Haute
    Critical     // Critique
}

public enum TicketType
{
    Maintenance,     // Maintenance
    Developpement,   // Développement
    Support          // Support
}
```

### Patterns Utilisés

#### 1. **MVC (Model-View-Controller)**

- **Models**: Entités et logique métier
- **Views**: Interface utilisateur (Razor)
- **Controllers**: Logique de contrôle et API

#### 2. **Repository Pattern** (via Entity Framework)

- Abstraction de l'accès aux données
- Requêtes LINQ pour la manipulation des données

#### 3. **Modal Dialog Pattern**

- Toutes les actions CRUD utilisent des modales
- Interface utilisateur non-intrusive
- Validation côté client et serveur

## 🔌 API Endpoints

### Tickets Controller

#### Vues

- `GET /Tickets/GestionDemandes` - Page principale de gestion

#### API AJAX

- `POST /Tickets/CreateModal` - Création de ticket
- `GET /Tickets/GetTicketData/{id}` - Récupération des données
- `POST /Tickets/EditModal` - Modification de ticket
- `POST /Tickets/DeleteModal` - Suppression individuelle
- `POST /Tickets/BulkDelete` - Suppression en lot
- `GET /Tickets/GetDropdownData` - Données pour les dropdowns

#### Exemples de Requêtes

**Création de Ticket**

```javascript
const formData = new FormData();
formData.append("Title", "Titre du ticket");
formData.append("Description", "Description détaillée");
formData.append("ClientId", "1");
formData.append("Status", "0"); // Open
formData.append("Priority", "1"); // Medium

fetch("/Tickets/CreateModal", {
  method: "POST",
  body: formData,
});
```

**Suppression en Lot**

```javascript
fetch("/Tickets/BulkDelete", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ ticketIds: [1, 2, 3] }),
});
```

## 💾 Base de Données

### Configuration SQL Server

```sql
-- Création de la base de données
CREATE DATABASE CliniSysDb;

-- Utilisation
USE CliniSysDb;

-- Les tables sont créées automatiquement par les migrations EF
```

### Tables Principales

#### Tickets

```sql
CREATE TABLE Tickets (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Title nvarchar(100) NOT NULL,
    Description nvarchar(max) NOT NULL,
    CreationDate datetime2 NOT NULL,
    Status int NOT NULL,
    Priority int NOT NULL,
    Type int NULL,
    ClientId int NOT NULL,
    UserId int NULL,
    TeamId int NULL,
    ModuleId int NULL,
    DurationDays int NULL,
    DurationMonths int NULL,
    RichTextComment nvarchar(max) NULL,
    AttachedFiles nvarchar(max) NULL,

    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TeamId) REFERENCES Teams(Id),
    FOREIGN KEY (ModuleId) REFERENCES Modules(Id)
);
```

### Migrations Disponibles

1. `InitialModel` - Structure de base
2. `AddUserPassword` - Ajout du mot de passe utilisateur
3. `AddEnhancedTicketFields` - Champs étendus pour les tickets
4. `RemoveUnwantedFields` - Suppression des champs obsolètes

### Commandes de Migration

```bash
# Créer une nouvelle migration
dotnet ef migrations add NomDeLaMigration

# Appliquer les migrations
dotnet ef database update

# Revenir à une migration spécifique
dotnet ef database update NomDeLaMigration

# Supprimer la dernière migration
dotnet ef migrations remove
```

## 🚀 Fonctionnalités Avancées

### 🎨 Système de Modales

Le système utilise des modales personnalisées pour toutes les interactions CRUD:

#### Caractéristiques des Modales

- **Overlay sombre** avec fermeture par clic extérieur
- **Validation en temps réel** côté client
- **Pré-remplissage automatique** pour l'édition
- **Éditeur de texte riche** intégré
- **Gestion des erreurs** avec messages utilisateur

#### Implémentation JavaScript

```javascript
function openModal() {
    // Chargement des données dropdown
    if (!dropdownData.clients) {
        await loadDropdownData();
    }

    // Affichage de la modale
    modal.style.display = 'flex';
    populateDropdowns();
}
```

### 📊 Export CSV Avancé

Le système d'export CSV inclut:

#### Fonctionnalités

- **Export sélectif** des tickets choisis
- **Toutes les colonnes** du tableau
- **Nom de fichier** avec timestamp
- **Encodage UTF-8** pour les caractères spéciaux

#### Format du Fichier

```csv
ID,Titre,Demandeur,Date,Statut,Priorité,Type,Utilisateur,Équipe,Module,Description
1,"Problème de connexion","Client A","2024-07-07","Open","High","Support","User1","Team1","Module1","Description détaillée"
```

### 🔍 Recherche Multi-Champs

La recherche fonctionne sur plusieurs champs simultanément:

#### Champs Recherchés

- Titre du ticket
- Nom du demandeur (client)
- Utilisateur assigné
- Équipe assignée
- Module assigné

#### Algorithme de Recherche

```javascript
function applyFilters() {
  const searchTerm = searchInput.value.toLowerCase();
  const searchableText = `${titre} ${demandeur} ${utilisateur} ${equipe} ${module}`;

  if (searchableText.includes(searchTerm)) {
    // Afficher la ligne
  }
}
```

### 🎯 Filtrage Intelligent

Le système de filtrage combine plusieurs critères:

#### Types de Filtres

1. **Recherche textuelle** - Multi-champs
2. **Filtres par énumération** - Statut, Type, Priorité
3. **Filtrage par dates** - Plage de dates
4. **Combinaison de filtres** - Tous les filtres ensemble

#### Mise à Jour Dynamique

- **Compteur de résultats** en temps réel
- **Sélection intelligente** (seulement les éléments visibles)
- **État des boutons** basé sur la sélection

## 🛠️ Dépannage

### Problèmes Courants

#### 1. Erreur de Connexion à la Base de Données

```
Microsoft.Data.SqlClient.SqlException: Cannot open database
```

**Solutions:**

- Vérifiez que SQL Server est démarré
- Contrôlez la chaîne de connexion dans `appsettings.json`
- Assurez-vous que la base de données existe
- Vérifiez les permissions utilisateur

#### 2. Migrations Non Appliquées

```
InvalidOperationException: No database provider has been configured
```

**Solutions:**

```bash
# Appliquer les migrations
dotnet ef database update

# Vérifier l'état des migrations
dotnet ef migrations list
```

#### 3. Modal Ne S'Ouvre Pas

**Symptômes:** Le bouton "+ Demande" ne fonctionne qu'une fois

**Solutions:**

- Vérifiez la console JavaScript (F12)
- Assurez-vous que `closeModal()` ne contient pas d'erreurs
- Contrôlez que les données dropdown se chargent correctement

#### 4. Filtres Ne Fonctionnent Pas

**Symptômes:** Les filtres ne masquent pas les lignes

**Solutions:**

- Vérifiez que les valeurs des énumérations correspondent
- Contrôlez la casse des comparaisons
- Assurez-vous que les event listeners sont attachés

### Logs et Débogage

#### Activation des Logs Détaillés

Dans `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

#### Console JavaScript

Ouvrez les outils de développement (F12) pour voir:

- Erreurs JavaScript
- Requêtes AJAX
- Réponses du serveur
- Messages de débogage

### Performance

#### Optimisations Recommandées

1. **Index sur les colonnes** fréquemment filtrées
2. **Pagination** pour les grandes listes
3. **Cache des données** dropdown
4. **Compression** des réponses HTTP

## 🤝 Contribution

### Guide de Contribution

#### 1. Fork et Clone

```bash
git clone https://github.com/VOTRE_USERNAME/GestionTicketClinisys.git
cd GestionTicketClinisys
```

#### 2. Créer une Branche

```bash
git checkout -b feature/nouvelle-fonctionnalite
```

#### 3. Développement

- Suivez les conventions de nommage C#
- Ajoutez des tests si nécessaire
- Documentez les nouvelles fonctionnalités

#### 4. Tests

```bash
# Lancer l'application
dotnet run

# Tester toutes les fonctionnalités
# - Création/édition/suppression de tickets
# - Filtres et recherche
# - Export CSV
# - Suppression en lot
```

#### 5. Commit et Push

```bash
git add .
git commit -m "feat: ajout de la nouvelle fonctionnalité"
git push origin feature/nouvelle-fonctionnalite
```

#### 6. Pull Request

Créez une Pull Request avec:

- Description détaillée des changements
- Captures d'écran si applicable
- Tests effectués

### Standards de Code

#### Conventions C#

- **PascalCase** pour les classes et méthodes
- **camelCase** pour les variables locales
- **Commentaires XML** pour les méthodes publiques
- **Async/await** pour les opérations asynchrones

#### Conventions JavaScript

- **camelCase** pour les variables et fonctions
- **const/let** au lieu de var
- **Arrow functions** quand approprié
- **Commentaires** pour la logique complexe

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 👥 Équipe

- **Développeur Principal**: [Votre Nom]
- **Architecture**: ASP.NET Core MVC + Entity Framework
- **Frontend**: HTML5/CSS3/JavaScript
- **Base de Données**: SQL Server

## 📞 Support

Pour toute question ou problème:

1. **Issues GitHub**: Créez une issue pour les bugs
2. **Discussions**: Utilisez les discussions pour les questions
3. **Email**: [votre.email@example.com]
4. **Documentation**: Consultez ce README

---

**🎫 Gestion Ticket Clinisys** - Système de gestion de tickets moderne et efficace pour environnements cliniques.

_Dernière mise à jour: Juillet 2024_
