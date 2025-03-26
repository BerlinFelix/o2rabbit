using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using o2rabbit.Core.Entities;
using o2rabbit.Core.Entities.Mappings;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace o2rabbit.Migrations.Context;

public class DefaultContext : DbContext
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    public virtual DbSet<Process> Processes { get; set; }
    public DbSet<ProcessComment> ProcessComments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<Space> Spaces { get; set; }
    public DbSet<SpaceComment> SpaceComments { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<StatusTransition> StatusTransitions { get; set; }
    public DbSet<Workflow> Workflows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Space

        modelBuilder.Entity<Space>()
            .ToTable("Spaces")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Space>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Space>()
            .HasMany(x => x.Comments)
            .WithOne(x => x.Space)
            .HasForeignKey(x => x.SpaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        modelBuilder.Entity<Space>()
            .HasMany(x => x.AttachableProcesses)
            .WithMany(x => x.PossibleSpaces)
            .UsingEntity<ProcessSpaceMapping>(
                l => l.HasOne<Process>().WithMany().HasForeignKey(e => e.ProcessId).OnDelete(DeleteBehavior.SetNull),
                r => r.HasOne<Space>().WithMany().HasForeignKey(e => e.SpaceId).OnDelete(DeleteBehavior.SetNull));


        modelBuilder.Entity<Space>()
            .HasMany(x => x.AttachedTickets)
            .WithOne(x => x.Space)
            .HasForeignKey(x => x.SpaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        #endregion

        #region Process

        modelBuilder.Entity<Process>()
            .ToTable("Processes")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Process>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Process>()
            .HasOne(x => x.Workflow)
            .WithOne(x => x.Process)
            .HasForeignKey<Workflow>(x => x.ProcessId)
            .IsRequired(true);

        modelBuilder.Entity<Process>()
            .HasMany(x => x.SubProcesses)
            .WithMany(x => x.PossibleParentProcesses)
            .UsingEntity<ProcessProcessMapping>(
                l => l.HasOne<Process>().WithMany().HasForeignKey(e => e.ChildId).OnDelete(DeleteBehavior.SetNull),
                r => r.HasOne<Process>().WithMany().HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.SetNull));

        #endregion

        #region Ticket

        modelBuilder.Entity<Ticket>()
            .ToTable("Tickets")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Ticket>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Ticket>()
            .HasMany(x => x.Children)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        modelBuilder.Entity<Ticket>()
            .HasOne(x => x.Process)
            .WithMany(p => p.Tickets)
            .HasForeignKey(x => x.ProcessId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(true);

        #endregion

        #region Workflow

        modelBuilder.Entity<Workflow>()
            .ToTable("Workflows")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Workflow>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Workflow>()
            .HasMany(x => x.Statuses)
            .WithOne(x => x.Workflow)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        #endregion

        #region Status

        modelBuilder.Entity<Status>()
            .ToTable("Statuses")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Status>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Status>()
            .HasMany(x => x.FromTransitions)
            .WithOne(x => x.FromStatus)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        modelBuilder.Entity<Status>()
            .HasMany(x => x.ToTransitions)
            .WithOne(x => x.ToStatus)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        #endregion

        #region StatusTransition

        modelBuilder.Entity<StatusTransition>()
            .ToTable("StatusTransitions")
            .HasKey(x => x.Id);

        modelBuilder.Entity<StatusTransition>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        #endregion

        #region SpaceComments

        modelBuilder.Entity<SpaceComment>()
            .ToTable("SpaceComments")
            .HasKey(x => x.Id);

        modelBuilder.Entity<SpaceComment>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<SpaceComment>()
            .HasOne(c => c.Space)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.SpaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        #endregion

        #region ProcessComment

        modelBuilder.Entity<ProcessComment>()
            .ToTable("ProcessComments")
            .HasKey(x => x.Id);

        modelBuilder.Entity<ProcessComment>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProcessComment>()
            .HasOne(c => c.Process)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.ProcessId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        #endregion

        #region TicketComment

        modelBuilder.Entity<TicketComment>()
            .ToTable("TicketComments")
            .HasKey(x => x.Id);

        modelBuilder.Entity<TicketComment>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        #endregion

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset?) || property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(new ValueConverter<DateTimeOffset?, DateTimeOffset?>(
                        d => d == null ? d : d.Value.ToUniversalTime(),
                        offset => offset
                    ));
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}