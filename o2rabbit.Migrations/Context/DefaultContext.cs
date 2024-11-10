using Microsoft.EntityFrameworkCore;
using o2rabbit.Core.Entities;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace o2rabbit.Migrations.Context;

public class DefaultContext : DbContext
{
    private readonly string? _connectionString;

    public DefaultContext(string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        _connectionString = connectionString;
    }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    public DbSet<Process> Processes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add ConnectionString directly when unit testing.
        if (_connectionString is not null)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Process>()
            .ToTable("Processes")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Process>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Process>()
            .HasMany(x => x.Children)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

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
            .WithMany()
            .HasForeignKey(x => x.ProcessId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        base.OnModelCreating(modelBuilder);
    }
}