using QTip.Application.Abstractions;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public class EmailDetectionServiceTests
{
    private readonly IEmailDetectionService _service = new EmailDetectionService();

    [Fact]
    public void Detect_ReturnsEmpty_WhenTextIsNullOrWhitespace()
    {
        IReadOnlyList<DetectedEmail> resultEmpty = _service.Detect(string.Empty);
        IReadOnlyList<DetectedEmail> resultWhitespace = _service.Detect("   ");

        Assert.Empty(resultEmpty);
        Assert.Empty(resultWhitespace);
    }

    [Fact]
    public void Detect_FindsAllEmailAddresses_WithCorrectValues()
    {
        const string text = "Contact john.doe@example.com and jane@test.org for details.";

        IReadOnlyList<DetectedEmail> result = _service.Detect(text);

        Assert.Equal(2, result.Count);
        Assert.Equal("john.doe@example.com", result[0].Value);
        Assert.Equal("jane@test.org", result[1].Value);
    }
}

