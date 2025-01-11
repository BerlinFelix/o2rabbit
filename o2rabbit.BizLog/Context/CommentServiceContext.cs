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

    public virtual DbSet<Comment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO configure logging
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
            .ToTable("Comments")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Comment>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(x => x.Id)
            .IsRequired(true);

        modelBuilder.Entity<Ticket>()
            .ToTable("Tickets")
            .HasKey(x => x.Id);

        base.OnModelCreating(modelBuilder);
    }
}