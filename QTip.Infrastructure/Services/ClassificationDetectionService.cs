using System.Text.RegularExpressions;
using QTip.Application.Abstractions;
using QTip.Domain.Enums;

namespace QTip.Infrastructure.Services;

/// <summary>
/// Detects configured PII and sensitive data types (email, IBAN, phone, tokens) within text.
/// </summary>
public sealed class ClassificationDetectionService : IClassificationDetectionService
{
    // Email detection (same semantics as the original EmailDetectionService).
    private static readonly Regex EmailRegex = new(
        pattern: @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // IBAN detection – simplified pattern for demo purposes.
    private static readonly Regex IbanRegex = new(
        pattern: @"\b[A-Z]{2}[0-9]{13,32}\b",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // Phone detection – basic international format, 7–15 digits with optional leading +.
    private static readonly Regex PhoneRegex = new(
        pattern: @"\b\+?[0-9]{7,15}\b",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // Security token detection – long opaque strings, 20+ allowed characters.
    private static readonly Regex SecurityTokenRegex = new(
        pattern: @"\b[A-Za-z0-9_\-]{20,}\b",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Runs all configured detectors over the text and returns a unified list of matches.
    /// </summary>
    public IReadOnlyList<DetectedClassification> Detect(string text)
    {
        List<DetectedClassification> classifications = new List<DetectedClassification>();

        AddMatchesForRegex(
            classifications,
            text,
            EmailRegex,
            PiiTag.PiiEmail.GetDescription());

        AddMatchesForRegex(
            classifications,
            text,
            IbanRegex,
            PiiTag.FinanceIban.GetDescription());

        AddMatchesForRegex(
            classifications,
            text,
            PhoneRegex,
            PiiTag.PiiPhone.GetDescription());

        AddMatchesForRegex(
            classifications,
            text,
            SecurityTokenRegex,
            PiiTag.SecurityToken.GetDescription());

        return classifications;
    }

    private static void AddMatchesForRegex(
        List<DetectedClassification> target,
        string text,
        Regex regex,
        string tag)
    {
        MatchCollection matches = regex.Matches(text);

        foreach (Match match in matches)
        {
            if (match.Success == false)
            {
                continue;
            }

            target.Add(
                new DetectedClassification(
                    tag,
                    match.Value,
                    match.Index,
                    match.Length));
        }
    }
}


