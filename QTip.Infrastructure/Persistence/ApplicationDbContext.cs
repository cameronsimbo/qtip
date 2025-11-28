using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Entities;
using QTip.Infrastructure.Persistence.Configurations;

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

        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new ClassificationRecordConfiguration());
    }
}


