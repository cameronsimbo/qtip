namespace QTip.Application.Abstractions;

/// <summary>
/// Detects configured classification types (e.g. email, IBAN, phone, tokens) within text.
/// </summary>
public sealed record DetectedClassification(string Tag, string Value, int StartIndex, int Length);

public interface IClassificationDetectionService
{
    /// <summary>
    /// Finds all configured classifications in the provided text.
    /// </summary>
    /// <param name="text">The text to scan.</param>
    /// <returns>A read-only list of detected classifications.</returns>
    IReadOnlyList<DetectedClassification> Detect(string text);
}
