namespace QTip.Application.Abstractions;

/// <summary>
/// Detects email addresses within a block of text and returns their values and positions.
/// </summary>
public sealed record DetectedEmail(string Value, int StartIndex, int Length);

public interface IEmailDetectionService
{
    /// <summary>
    /// Finds all email addresses in the provided text.
    /// </summary>
    /// <param name="text">The text to scan for email addresses.</param>
    /// <returns>A read-only list of detected email values and their positions.</returns>
    IReadOnlyList<DetectedEmail> Detect(string text);
}


