using MediatR;

namespace QTip.Application.Features.Statistics;

public sealed record GetTotalPiiEmailCountQuery : IRequest<GetTotalPiiEmailCountResult>;

public sealed record GetTotalPiiEmailCountResult(long TotalPiiEmails);


