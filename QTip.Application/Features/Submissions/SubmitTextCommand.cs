using MediatR;

namespace QTip.Application.Features.Submissions;

public sealed record SubmitTextCommand(string Text) : IRequest<SubmitTextResult>;

public sealed record SubmitTextResult(string TokenizedText, int DetectedEmailCount);