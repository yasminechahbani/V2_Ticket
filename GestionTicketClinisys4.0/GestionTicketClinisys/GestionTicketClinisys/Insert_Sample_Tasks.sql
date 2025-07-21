-- Insert Sample Tasks for Testing Enhanced Task Dashboard
-- This script creates sample tasks for the admin user and other users

USE CliniSysDb;
GO

-- Get some ticket IDs and user IDs for reference
DECLARE @AdminUserId INT = 10; -- admin user
DECLARE @MarieUserId INT = 11; -- marie.martin
DECLARE @PierreUserId INT = 12; -- pierre.durand

-- Insert sample tasks for admin user (current logged in user)
INSERT INTO TaskItems (
    Title, 
    Description, 
    CreatedAt, 
    Status, 
    StartDate, 
    DueDate, 
    Priority, 
    EstimatedHours, 
    ActualHours, 
    Progress, 
    KanbanOrder, 
    TicketId, 
    AssignedUserId
) VALUES
-- Not Started Tasks
('Setup Database Backup System', 'Configure automated backup system for the main database with daily incremental backups', GETDATE(), 0, GETDATE(), DATEADD(day, 7, GETDATE()), 4, 16, 0, 0, 1, 1, @AdminUserId),
('Review Security Protocols', 'Conduct comprehensive review of current security protocols and update documentation', GETDATE(), 0, GETDATE(), DATEADD(day, 5, GETDATE()), 3, 12, 0, 0, 2, 2, @AdminUserId),
('Plan System Migration', 'Create detailed plan for migrating legacy systems to new infrastructure', GETDATE(), 0, DATEADD(day, 2, GETDATE()), DATEADD(day, 14, GETDATE()), 3, 24, 0, 0, 3, 3, @AdminUserId),

-- In Progress Tasks
('Implement User Authentication', 'Develop and implement enhanced user authentication system with 2FA support', DATEADD(day, -3, GETDATE()), 1, DATEADD(day, -3, GETDATE()), DATEADD(day, 4, GETDATE()), 4, 20, 12, 60, 1, 4, @AdminUserId),
('Database Performance Optimization', 'Optimize database queries and improve overall system performance', DATEADD(day, -5, GETDATE()), 1, DATEADD(day, -5, GETDATE()), DATEADD(day, 2, GETDATE()), 3, 18, 14, 75, 2, 5, @AdminUserId),
('API Documentation Update', 'Update API documentation with new endpoints and authentication methods', DATEADD(day, -2, GETDATE()), 1, DATEADD(day, -2, GETDATE()), DATEADD(day, 3, GETDATE()), 2, 8, 4, 50, 3, 6, @AdminUserId),

-- Completed Tasks
('Server Maintenance', 'Performed scheduled maintenance on production servers including OS updates', DATEADD(day, -10, GETDATE()), 2, DATEADD(day, -10, GETDATE()), DATEADD(day, -8, GETDATE()), 2, 6, 6, 100, 1, 7, @AdminUserId),
('User Training Session', 'Conducted training session for new users on system functionality', DATEADD(day, -7, GETDATE()), 2, DATEADD(day, -7, GETDATE()), DATEADD(day, -6, GETDATE()), 1, 4, 4, 100, 2, 8, @AdminUserId),

-- Cancelled Task
('Legacy System Integration', 'Integration with old legacy system - cancelled due to compatibility issues', DATEADD(day, -15, GETDATE()), 3, DATEADD(day, -15, GETDATE()), DATEADD(day, -10, GETDATE()), 2, 16, 8, 0, 1, 9, @AdminUserId);

-- Insert some tasks for other users (for team collaboration testing)
INSERT INTO TaskItems (
    Title, 
    Description, 
    CreatedAt, 
    Status, 
    StartDate, 
    DueDate, 
    Priority, 
    EstimatedHours, 
    ActualHours, 
    Progress, 
    KanbanOrder, 
    TicketId, 
    AssignedUserId
) VALUES
-- Tasks for Marie (same tickets as admin for collaboration)
('Frontend Development', 'Develop responsive frontend interface for the new authentication system', GETDATE(), 1, GETDATE(), DATEADD(day, 6, GETDATE()), 3, 20, 8, 40, 1, 4, @MarieUserId),
('UI/UX Design Review', 'Review and improve user interface design based on user feedback', DATEADD(day, -1, GETDATE()), 0, DATEADD(day, 1, GETDATE()), DATEADD(day, 8, GETDATE()), 2, 12, 0, 0, 1, 1, @MarieUserId),

-- Tasks for Pierre (same tickets for collaboration)
('Database Schema Updates', 'Update database schema to support new authentication features', DATEADD(day, -2, GETDATE()), 1, DATEADD(day, -2, GETDATE()), DATEADD(day, 5, GETDATE()), 4, 16, 10, 65, 1, 4, @PierreUserId),
('Performance Testing', 'Conduct comprehensive performance testing on optimized database queries', GETDATE(), 0, DATEADD(day, 1, GETDATE()), DATEADD(day, 4, GETDATE()), 3, 10, 0, 0, 1, 5, @PierreUserId);

-- Update some tasks with additional metadata
UPDATE TaskItems SET 
    Tags = '["authentication", "security", "critical"]',
    Comments = '[{"user": "admin", "date": "2024-01-20", "text": "High priority task - needs to be completed ASAP"}]'
WHERE Title = 'Setup Database Backup System';

UPDATE TaskItems SET 
    Tags = '["frontend", "ui", "responsive"]',
    Comments = '[{"user": "marie.martin", "date": "2024-01-19", "text": "Working on mobile responsiveness first"}]'
WHERE Title = 'Frontend Development';

UPDATE TaskItems SET 
    Tags = '["database", "performance", "optimization"]',
    Comments = '[{"user": "admin", "date": "2024-01-18", "text": "Great progress so far!"}, {"user": "pierre.durand", "date": "2024-01-19", "text": "Query optimization showing 40% improvement"}]'
WHERE Title = 'Database Performance Optimization';

PRINT 'Sample tasks inserted successfully!';
PRINT 'Tasks created for admin user (ID: 10) - 9 tasks';
PRINT 'Tasks created for marie.martin (ID: 11) - 2 tasks';  
PRINT 'Tasks created for pierre.durand (ID: 12) - 2 tasks';
PRINT 'Total: 13 tasks with various statuses and priorities';
PRINT '';
PRINT 'Task distribution:';
PRINT '- Not Started: 3 tasks';
PRINT '- In Progress: 3 tasks';  
PRINT '- Completed: 2 tasks';
PRINT '- Cancelled: 1 task';
PRINT '';
PRINT 'You can now test the enhanced Task Dashboard with:';
PRINT '- Kanban drag-and-drop functionality';
PRINT '- Gantt chart timeline view';
PRINT '- Team collaboration features';
PRINT '- Summary cards and statistics';
