using Microsoft.EntityFrameworkCore;
using QTip.Domain.Entities;

namespace QTip.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Submission> Submissions { get; }

    DbSet<ClassificationRecord> ClassificationRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


