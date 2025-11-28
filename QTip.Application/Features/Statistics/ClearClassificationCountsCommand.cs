using MediatR;

namespace QTip.Application.Features.Statistics;

public sealed record ClearClassificationCountsCommand : IRequest<ClearClassificationCountsResult>;

public sealed record ClearClassificationCountsResult(bool Success);


