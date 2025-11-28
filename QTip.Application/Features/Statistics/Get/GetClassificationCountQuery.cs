using MediatR;

namespace QTip.Application.Features.Statistics.Get;

public sealed record GetClassificationCountQuery : IRequest<GetClassificationCountResult>;

public sealed record GetClassificationCountResult(
    long TotalPiiEmails,
    long TotalFinanceIbans,
    long TotalPiiPhones,
    long TotalSecurityTokens,
    long LastSubmissionPiiEmails,
    long LastSubmissionFinanceIbans,
    long LastSubmissionPiiPhones,
    long LastSubmissionSecurityTokens);