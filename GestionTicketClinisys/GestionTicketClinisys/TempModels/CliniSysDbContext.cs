using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketClinisys.TempModels;

public partial class CliniSysDbContext : DbContext
{
    public CliniSysDbContext()
    {
    }

    public CliniSysDbContext(DbContextOptions<CliniSysDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<TaskItem> TaskItems { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=CliniSysDb;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasIndex(e => e.AssignedUserId, "IX_TaskItems_AssignedUserId");

            entity.HasIndex(e => e.TicketId, "IX_TaskItems_TicketId");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.TaskItems).HasForeignKey(d => d.AssignedUserId);

            entity.HasOne(d => d.Ticket).WithMany(p => p.TaskItems).HasForeignKey(d => d.TicketId);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.Name).HasDefaultValue("");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_Tickets_ClientId");

            entity.HasIndex(e => e.ModuleId, "IX_Tickets_ModuleId");

            entity.HasIndex(e => e.TeamId, "IX_Tickets_TeamId");

            entity.HasIndex(e => e.UserId, "IX_Tickets_UserId");

            entity.Property(e => e.Designation).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Client).WithMany(p => p.Tickets).HasForeignKey(d => d.ClientId);

            entity.HasOne(d => d.Module).WithMany(p => p.Tickets).HasForeignKey(d => d.ModuleId);

            entity.HasOne(d => d.Team).WithMany(p => p.Tickets).HasForeignKey(d => d.TeamId);

            entity.HasOne(d => d.User).WithMany(p => p.Tickets).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.TeamId, "IX_Users_TeamId");

            entity.Property(e => e.Password).HasDefaultValue("");

            entity.HasOne(d => d.Team).WithMany(p => p.Users).HasForeignKey(d => d.TeamId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
