using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using o2rabbit.BizLog.Options.CommentServiceContext;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Context;

public class CommentServiceContext : DbContext
{
    private readonly IOptions<CommentServiceContextOptions> _options;

    public CommentServiceContext(IOptions<CommentServiceContextOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

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