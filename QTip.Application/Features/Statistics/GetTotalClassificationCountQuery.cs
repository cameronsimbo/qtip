using MediatR;

namespace QTip.Application.Features.Statistics;

public sealed record GetTotalClassificationCountQuery : IRequest<GetTotalClassificationCountResult>;

public sealed record GetTotalClassificationCountResult(
    long TotalPiiEmails,
    long TotalFinanceIbans,
    long TotalPiiPhones,
    long TotalSecurityTokens);


