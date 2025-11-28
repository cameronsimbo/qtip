using MediatR;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Domain.Enums;

namespace QTip.Application.Features.Statistics.Get;

public sealed class GetClassificationCountQueryHandler
    : IRequestHandler<GetClassificationCountQuery, GetClassificationCountResult>
{
    private readonly IApplicationDbContext _dbContext;

    public GetClassificationCountQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetClassificationCountResult> Handle(
        GetClassificationCountQuery request,
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

        Guid? lastSubmissionId = await _dbContext.Submissions
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        long lastSubmissionPiiEmails = 0;
        long lastSubmissionFinanceIbans = 0;
        long lastSubmissionPiiPhones = 0;
        long lastSubmissionSecurityTokens = 0;

        if (lastSubmissionId.HasValue)
        {
            Guid submissionId = lastSubmissionId.Value;

            lastSubmissionPiiEmails = await _dbContext.ClassificationRecords
                .Where(x => x.SubmissionId == submissionId && x.Tag == emailTag)
                .LongCountAsync(cancellationToken);

            lastSubmissionFinanceIbans = await _dbContext.ClassificationRecords
                .Where(x => x.SubmissionId == submissionId && x.Tag == ibanTag)
                .LongCountAsync(cancellationToken);

            lastSubmissionPiiPhones = await _dbContext.ClassificationRecords
                .Where(x => x.SubmissionId == submissionId && x.Tag == phoneTag)
                .LongCountAsync(cancellationToken);

            lastSubmissionSecurityTokens = await _dbContext.ClassificationRecords
                .Where(x => x.SubmissionId == submissionId && x.Tag == tokenTag)
                .LongCountAsync(cancellationToken);
        }

        return new GetClassificationCountResult(
            totalPiiEmails,
            totalFinanceIbans,
            totalPiiPhones,
            totalSecurityTokens,
            lastSubmissionPiiEmails,
            lastSubmissionFinanceIbans,
            lastSubmissionPiiPhones,
            lastSubmissionSecurityTokens);
    }
}