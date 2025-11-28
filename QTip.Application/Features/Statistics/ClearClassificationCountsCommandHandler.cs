using MediatR;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Entities;

namespace QTip.Application.Features.Statistics;

public sealed class ClearClassificationCountsCommandHandler
    : IRequestHandler<ClearClassificationCountsCommand, ClearClassificationCountsResult>
{
    private readonly IApplicationDbContext _dbContext;

    public ClearClassificationCountsCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClearClassificationCountsResult> Handle(
        ClearClassificationCountsCommand request,
        CancellationToken cancellationToken)
    {
        List<ClassificationRecord> classifications =
            await _dbContext.ClassificationRecords.ToListAsync(cancellationToken);
        List<Submission> submissions =
            await _dbContext.Submissions.ToListAsync(cancellationToken);

        if (classifications.Count > 0)
        {
            _dbContext.ClassificationRecords.RemoveRange(classifications);
        }

        if (submissions.Count > 0)
        {
            _dbContext.Submissions.RemoveRange(submissions);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ClearClassificationCountsResult(true);
    }
}


