namespace QTip.Application.Abstractions;

public sealed record DetectedEmail(string Value, int StartIndex, int Length);

public interface IEmailDetectionService
{
    IReadOnlyList<DetectedEmail> Detect(string text);
}


