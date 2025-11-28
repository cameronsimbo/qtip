using System.Text.RegularExpressions;
using QTip.Application.Abstractions;

namespace QTip.Infrastructure.Services;

public sealed class EmailDetectionService : IEmailDetectionService
{
    // Simple but robust email regex suitable for this challenge scope.
    private static readonly Regex EmailRegex = new(
        pattern: @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public IReadOnlyList<DetectedEmail> Detect(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<DetectedEmail>();
        }

        MatchCollection matches = EmailRegex.Matches(text);
        List<DetectedEmail> results = new List<DetectedEmail>(matches.Count);

        foreach (Match match in matches)
        {
            if (!match.Success)
            {
                continue;
            }

            results.Add(new DetectedEmail(match.Value, match.Index, match.Length));
        }

        return results;
    }
}


