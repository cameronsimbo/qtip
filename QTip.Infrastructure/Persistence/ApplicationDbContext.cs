using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Entities;

namespace QTip.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Submission> Submissions => Set<Submission>();

    public DbSet<ClassificationRecord> ClassificationRecords => Set<ClassificationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Submission>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TokenizedText)
                .IsRequired();
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        modelBuilder.Entity<ClassificationRecord>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.OriginalValue)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Tag)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => x.Tag);

            builder.HasOne<Submission>()
                .WithMany()
                .HasForeignKey(x => x.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}


