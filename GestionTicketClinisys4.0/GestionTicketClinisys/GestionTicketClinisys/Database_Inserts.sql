-- =====================================================
-- GestionTicketClinisys Database Insert Scripts
-- =====================================================
-- This file contains comprehensive INSERT statements for all tables
-- Execute in order to respect foreign key dependencies

USE CliniSysDb;
GO

-- =====================================================
-- 1. TEAMS TABLE
-- =====================================================
-- Clear existing data (optional - uncomment if needed)
-- DELETE FROM TaskItems;
-- DELETE FROM Tickets;
-- DELETE FROM Users;
-- DELETE FROM Clients;
-- DELETE FROM Modules;
-- DELETE FROM Teams;

INSERT INTO Teams (Name) VALUES
('Équipe Développement'),
('Équipe Support Technique'),
('Équipe Infrastructure'),
('Équipe Sécurité'),
('Équipe Formation'),
('Équipe Qualité'),
('Équipe Maintenance'),
('Équipe Intégration');

-- =====================================================
-- 2. MODULES TABLE
-- =====================================================
INSERT INTO Modules (Name) VALUES
('Gestion Patients'),
('Facturation'),
('Pharmacie'),
('Laboratoire'),
('Radiologie'),
('Bloc Opératoire'),
('Urgences'),
('Hospitalisation'),
('Consultations'),
('Rendez-vous'),
('Ressources Humaines'),
('Comptabilité'),
('Stock & Inventaire'),
('Sécurité & Accès'),
('Reporting'),
('Interface HL7'),
('Sauvegarde'),
('Administration Système');

-- =====================================================
-- 3. CLIENTS TABLE
-- =====================================================
INSERT INTO Clients (Name, Email) VALUES
('Hôpital Central de Tunis', 'contact@hopital-central.tn'),
('Clinique Internationale Carthage', 'info@carthage-clinic.tn'),
('Centre Médical Espoir', 'admin@centre-espoir.tn'),
('Polyclinique du Nord', 'contact@poly-nord.tn'),
('Hôpital Universitaire Mongi Slim', 'direction@mongi-slim.tn'),
('Clinique Pasteur', 'info@pasteur-clinic.tn'),
('Centre de Santé Mentale', 'contact@sante-mentale.tn'),
('Hôpital Pédiatrique Béchir Hamza', 'admin@bechir-hamza.tn'),
('Clinique Ophtalmologique', 'info@ophtalmo-clinic.tn'),
('Centre de Cardiologie Avancée', 'contact@cardio-center.tn'),
('Hôpital Régional de Sfax', 'direction@hopital-sfax.tn'),
('Clinique Dentaire Moderne', 'info@dentaire-moderne.tn');

-- =====================================================
-- 4. USERS TABLE
-- =====================================================
INSERT INTO Users (UserName, Email, Role, Password, FirstName, LastName, Phone, Address, DateOfBirth, Nationality, Position, Department, Manager, IsActive, TeamId) VALUES
('admin', 'admin@clinisys.tn', 'Admin', 'admin123', 'Ahmed', 'Ben Ali', '+216 20 123 456', '123 Avenue Habib Bourguiba, Tunis', '1985-03-15', 'Tunisienne', 'Administrateur Système', 'IT', NULL, 1, 1),
('dev.lead', 'dev.lead@clinisys.tn', 'Developer', 'dev123', 'Fatma', 'Trabelsi', '+216 22 234 567', '456 Rue de la République, Tunis', '1988-07-22', 'Tunisienne', 'Chef Développeur', 'Développement', 'Ahmed Ben Ali', 1, 1),
('support.tech', 'support@clinisys.tn', 'Support', 'support123', 'Mohamed', 'Karray', '+216 25 345 678', '789 Boulevard du 7 Novembre, Ariana', '1990-11-08', 'Tunisienne', 'Technicien Support', 'Support', 'Ahmed Ben Ali', 1, 2),
('qa.manager', 'qa@clinisys.tn', 'QA', 'qa123', 'Leila', 'Mansouri', '+216 28 456 789', '321 Avenue de la Liberté, Sfax', '1987-05-14', 'Tunisienne', 'Responsable Qualité', 'Qualité', 'Ahmed Ben Ali', 1, 6),
('dev.junior', 'junior.dev@clinisys.tn', 'Developer', 'junior123', 'Youssef', 'Hamdi', '+216 29 567 890', '654 Rue Ibn Khaldoun, Sousse', '1995-09-30', 'Tunisienne', 'Développeur Junior', 'Développement', 'Fatma Trabelsi', 1, 1),
('infra.admin', 'infra@clinisys.tn', 'Admin', 'infra123', 'Sarra', 'Bouaziz', '+216 21 678 901', '987 Avenue Mongi Bali, Monastir', '1989-12-03', 'Tunisienne', 'Administrateur Infrastructure', 'Infrastructure', 'Ahmed Ben Ali', 1, 3),
('security.expert', 'security@clinisys.tn', 'Security', 'security123', 'Karim', 'Jebali', '+216 23 789 012', '147 Rue de Marseille, Bizerte', '1986-04-18', 'Tunisienne', 'Expert Sécurité', 'Sécurité', 'Ahmed Ben Ali', 1, 4),
('trainer', 'formation@clinisys.tn', 'Trainer', 'trainer123', 'Amina', 'Sassi', '+216 26 890 123', '258 Avenue Taieb Mhiri, Gabès', '1991-08-25', 'Tunisienne', 'Formatrice', 'Formation', 'Ahmed Ben Ali', 1, 5);

-- =====================================================
-- 5. TICKETS TABLE
-- =====================================================
INSERT INTO Tickets (Title, Description, CreationDate, Status, Priority, DurationDays, DurationMonths, Type, RichTextComment, ClientId, TeamId, ModuleId) VALUES
('Problème de connexion base de données', 'Les utilisateurs ne peuvent pas se connecter au module de gestion des patients depuis ce matin.', '2024-01-15 08:30:00', 1, 3, 2, 0, 2, 'Problème critique affectant tous les utilisateurs du module patients.', 1, 2, 1),
('Nouvelle fonctionnalité facturation', 'Développement d''une nouvelle interface pour la facturation automatique des actes médicaux.', '2024-01-14 14:20:00', 0, 2, 15, 1, 1, 'Demande de développement pour améliorer l''efficacité de la facturation.', 2, 1, 2),
('Formation utilisateurs module pharmacie', 'Organisation d''une session de formation pour les nouveaux utilisateurs du module pharmacie.', '2024-01-13 10:15:00', 0, 1, 5, 0, 2, 'Formation prévue pour 20 utilisateurs sur 2 jours.', 3, 5, 3),
('Optimisation performances laboratoire', 'Le module laboratoire est lent lors de la saisie des résultats d''analyses.', '2024-01-12 16:45:00', 1, 2, 10, 0, 0, 'Problème de performance identifié lors des heures de pointe.', 4, 3, 4),
('Mise à jour sécurité système', 'Application des derniers correctifs de sécurité sur l''ensemble du système.', '2024-01-11 09:00:00', 2, 3, 3, 0, 0, 'Mise à jour critique pour la sécurité du système.', 5, 4, 14),
('Bug affichage radiologie', 'Les images radiologiques ne s''affichent pas correctement dans certains navigateurs.', '2024-01-10 11:30:00', 1, 2, 7, 0, 2, 'Problème d''affichage spécifique aux navigateurs Firefox et Safari.', 6, 1, 5),
('Sauvegarde automatique défaillante', 'Le système de sauvegarde automatique ne fonctionne plus depuis 3 jours.', '2024-01-09 07:45:00', 0, 3, 1, 0, 0, 'Problème critique de sauvegarde nécessitant une intervention immédiate.', 7, 3, 17),
('Interface HL7 hôpital partenaire', 'Configuration d''une nouvelle interface HL7 pour l''échange de données avec un hôpital partenaire.', '2024-01-08 13:20:00', 0, 2, 20, 1, 1, 'Projet d''intégration avec l''Hôpital Universitaire de Sousse.', 8, 8, 16),
('Rapport mensuel activité', 'Génération automatique des rapports mensuels d''activité pour la direction.', '2024-01-07 15:10:00', 1, 1, 12, 0, 1, 'Développement d''un module de reporting automatisé.', 9, 1, 15),
('Maintenance serveur principal', 'Maintenance préventive du serveur principal prévue pour le weekend.', '2024-01-06 12:00:00', 0, 2, 2, 0, 0, 'Maintenance programmée incluant mise à jour OS et vérification hardware.', 10, 7, 18);

-- =====================================================
-- 6. TASKITEMS TABLE
-- =====================================================
INSERT INTO TaskItems (Title, Description, CreatedAt, Status, TicketId, AssignedUserId) VALUES
('Diagnostic problème BDD', 'Analyser les logs de connexion à la base de données', '2024-01-15 09:00:00', 1, 1, 3),
('Redémarrage services', 'Redémarrer les services de base de données', '2024-01-15 09:30:00', 0, 1, 6),
('Analyse des besoins', 'Recueillir les spécifications détaillées pour la facturation', '2024-01-14 15:00:00', 2, 2, 2),
('Conception interface', 'Créer les maquettes de la nouvelle interface', '2024-01-14 16:00:00', 0, 2, 5),
('Préparation matériel formation', 'Préparer les supports de formation et l''environnement de test', '2024-01-13 11:00:00', 1, 3, 8),
('Planification sessions', 'Organiser le planning des sessions de formation', '2024-01-13 14:00:00', 0, 3, 8),
('Analyse performance', 'Identifier les goulots d''étranglement dans le module laboratoire', '2024-01-12 17:00:00', 1, 4, 6),
('Optimisation requêtes', 'Optimiser les requêtes SQL du module', '2024-01-12 18:00:00', 0, 4, 2),
('Téléchargement correctifs', 'Télécharger et tester les correctifs de sécurité', '2024-01-11 09:30:00', 2, 5, 7),
('Application patches', 'Appliquer les correctifs sur l''environnement de production', '2024-01-11 14:00:00', 1, 5, 7),
('Test navigateurs', 'Tester l''affichage sur différents navigateurs', '2024-01-10 12:00:00', 1, 6, 5),
('Correction CSS', 'Corriger les problèmes de compatibilité CSS', '2024-01-10 15:00:00', 0, 6, 5),
('Vérification logs sauvegarde', 'Analyser les logs du système de sauvegarde', '2024-01-09 08:00:00', 2, 7, 6),
('Réparation service', 'Réparer le service de sauvegarde automatique', '2024-01-09 10:00:00', 1, 7, 6),
('Configuration serveur HL7', 'Configurer le serveur d''interface HL7', '2024-01-08 14:00:00', 1, 8, 6),
('Test connexion', 'Tester la connexion avec l''hôpital partenaire', '2024-01-08 16:00:00', 0, 8, 3),
('Conception modèle rapport', 'Concevoir le modèle de rapport mensuel', '2024-01-07 16:00:00', 1, 9, 2),
('Développement générateur', 'Développer le générateur automatique de rapports', '2024-01-07 17:00:00', 0, 9, 5),
('Planification maintenance', 'Planifier les étapes de la maintenance serveur', '2024-01-06 13:00:00', 2, 10, 6),
('Préparation environnement', 'Préparer l''environnement de basculement', '2024-01-06 14:00:00', 1, 10, 7);

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Uncomment these queries to verify the data insertion

-- SELECT COUNT(*) as 'Teams Count' FROM Teams;
-- SELECT COUNT(*) as 'Modules Count' FROM Modules;
-- SELECT COUNT(*) as 'Clients Count' FROM Clients;
-- SELECT COUNT(*) as 'Users Count' FROM Users;
-- SELECT COUNT(*) as 'Tickets Count' FROM Tickets;
-- SELECT COUNT(*) as 'TaskItems Count' FROM TaskItems;

-- =====================================================
-- SAMPLE QUERIES FOR TESTING
-- =====================================================
-- Get tickets with related data
-- SELECT t.Title, c.Name as Client, tm.Name as Team, m.Name as Module, t.Status, t.Priority
-- FROM Tickets t
-- LEFT JOIN Clients c ON t.ClientId = c.Id
-- LEFT JOIN Teams tm ON t.TeamId = tm.Id
-- LEFT JOIN Modules m ON t.ModuleId = m.Id;

-- Get tasks with assigned users
-- SELECT ti.Title, ti.Status, u.FirstName + ' ' + u.LastName as AssignedUser, t.Title as TicketTitle
-- FROM TaskItems ti
-- JOIN Users u ON ti.AssignedUserId = u.Id
-- JOIN Tickets t ON ti.TicketId = t.Id;

PRINT 'Database insert completed successfully!';
PRINT 'Data inserted:';
PRINT '- 8 Teams';
PRINT '- 18 Modules';
PRINT '- 12 Clients';
PRINT '- 8 Users';
PRINT '- 10 Tickets';
PRINT '- 20 TaskItems';
