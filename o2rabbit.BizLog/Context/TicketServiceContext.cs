using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.TicketServiceContext;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Context;

public class TicketServiceContext : DbContext
{
    private readonly IOptions<TicketServiceContextOptions> _options;

    //For mocking
    public TicketServiceContext()
    {
    }

    public TicketServiceContext(IOptions<TicketServiceContextOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketComment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO configure logging
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine);
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
            .IsRequired(false);

        modelBuilder.Entity<TicketComment>()
            .ToTable("Comments")
            .HasKey(x => x.Id);

        modelBuilder.Entity<TicketComment>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.TicketId)
            .IsRequired(true);

        modelBuilder.Entity<TicketComment>()
            .Property(c => c.DeletedAt)
            .HasConversion(d => d == null ? d : d.Value.ToUniversalTime(), offset => offset);

        base.OnModelCreating(modelBuilder);
    }
}