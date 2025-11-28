using Microsoft.Extensions.DependencyInjection;
using QTip.Application.Abstractions;
using QTip.Domain.Enums;

namespace QTip.Tests;

public class ClassificationDetectionServiceTests : TestBase
{
    private readonly IClassificationDetectionService _service;

    public ClassificationDetectionServiceTests()
    {
        ServiceProvider provider = CreateServiceProvider();
        _service = provider.GetRequiredService<IClassificationDetectionService>();
    }

    [Fact]
    public void Detect_ReturnsEmpty_WhenTextIsNullOrWhitespace()
    {
        IReadOnlyList<DetectedClassification> resultEmpty = _service.Detect(string.Empty);
        IReadOnlyList<DetectedClassification> resultWhitespace = _service.Detect("   ");

        Assert.Empty(resultEmpty);
        Assert.Empty(resultWhitespace);
    }

    [Fact]
    public void Detect_FindsAllEmailAddresses_WithCorrectValues()
    {
        const string text = "Contact john.doe@example.com and jane@test.org for details.";

        IReadOnlyList<DetectedClassification> allResults = _service.Detect(text);
        string emailTag = PiiTag.PiiEmail.GetDescription();

        List<DetectedClassification> result = allResults
            .Where(x => x.Tag == emailTag)
            .OrderBy(x => x.StartIndex)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("john.doe@example.com", result[0].Value);
        Assert.Equal("jane@test.org", result[1].Value);
    }

    [Fact]
    public void Detect_FindsIbanNumbers_WithCorrectTag()
    {
        const string text = "Primary IBAN is GB12TEST1234567890123 and backup is GB99TEST0000000000123.";

        IReadOnlyList<DetectedClassification> allResults = _service.Detect(text);
        string ibanTag = PiiTag.FinanceIban.GetDescription();

        List<DetectedClassification> result = allResults
            .Where(x => x.Tag == ibanTag)
            .OrderBy(x => x.StartIndex)
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void Detect_FindsPhoneNumbers_WithCorrectTag()
    {
        const string text = "Call 441234567890 or 0123456789 for support.";

        IReadOnlyList<DetectedClassification> allResults = _service.Detect(text);
        string phoneTag = PiiTag.PiiPhone.GetDescription();

        List<DetectedClassification> result = allResults
            .Where(x => x.Tag == phoneTag)
            .OrderBy(x => x.StartIndex)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("441234567890", result[0].Value);
        Assert.Equal("0123456789", result[1].Value);
    }

    [Fact]
    public void Detect_FindsSecurityTokens_WithCorrectTag()
    {
        const string text = "Token ABCDEFGHIJKLMNOPQRST and token ZYXWVUTSRQPONMLKJIHG exist.";

        IReadOnlyList<DetectedClassification> allResults = _service.Detect(text);
        string tokenTag = PiiTag.SecurityToken.GetDescription();

        List<DetectedClassification> result = allResults
            .Where(x => x.Tag == tokenTag)
            .OrderBy(x => x.StartIndex)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("ABCDEFGHIJKLMNOPQRST", result[0].Value);
        Assert.Equal("ZYXWVUTSRQPONMLKJIHG", result[1].Value);
    }
}

