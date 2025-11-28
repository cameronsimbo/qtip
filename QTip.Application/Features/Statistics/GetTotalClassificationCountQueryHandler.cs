using MediatR;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Enums;

namespace QTip.Application.Features.Statistics;

public sealed class GetTotalClassificationCountQueryHandler
    : IRequestHandler<GetTotalClassificationCountQuery, GetTotalClassificationCountResult>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTotalClassificationCountQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetTotalClassificationCountResult> Handle(
        GetTotalClassificationCountQuery request,
        CancellationToken cancellationToken)
    {
        string emailTag = PiiTag.PiiEmail.GetDescription();
        string ibanTag = PiiTag.FinanceIban.GetDescription();
        string phoneTag = PiiTag.PiiPhone.GetDescription();
        string tokenTag = PiiTag.SecurityToken.GetDescription();

        long totalPiiEmails = await _dbContext.ClassificationRecords
            .Where(x => x.Tag == emailTag)
            .LongCountAsync(cancellationToken);

        long totalFinanceIbans = await _dbContext.ClassificationRecords
            .Where(x => x.Tag == ibanTag)
            .LongCountAsync(cancellationToken);

        long totalPiiPhones = await _dbContext.ClassificationRecords
            .Where(x => x.Tag == phoneTag)
            .LongCountAsync(cancellationToken);

        long totalSecurityTokens = await _dbContext.ClassificationRecords
            .Where(x => x.Tag == tokenTag)
            .LongCountAsync(cancellationToken);

        return new GetTotalClassificationCountResult(
            totalPiiEmails,
            totalFinanceIbans,
            totalPiiPhones,
            totalSecurityTokens);
    }
}