using MediatR;

namespace QTip.Application.Features.Statistics.Clear;

public sealed record ClearClassificationCountsCommand : IRequest<ClearClassificationCountsResult>;

public sealed record ClearClassificationCountsResult(bool Success);