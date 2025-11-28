using MediatR;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Enums;

namespace QTip.Application.Features.Statistics;

public sealed class GetTotalPiiEmailCountQueryHandler
    : IRequestHandler<GetTotalPiiEmailCountQuery, GetTotalPiiEmailCountResult>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTotalPiiEmailCountQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetTotalPiiEmailCountResult> Handle(
        GetTotalPiiEmailCountQuery request,
        CancellationToken cancellationToken)
    {
        long count = await _dbContext.ClassificationRecords
            .Where(x => x.Tag == PiiTag.PiiEmail.GetDescription())
            .LongCountAsync(cancellationToken);

        return new GetTotalPiiEmailCountResult(count);
    }
}


