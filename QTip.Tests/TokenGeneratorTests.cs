using System.Text.RegularExpressions;
using QTip.Application.Abstractions;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public class TokenGeneratorTests
{
    private readonly ITokenGenerator _tokenGenerator = new TokenGenerator();

    private static readonly Regex TokenPattern = new(
        pattern: @"^\{\{TKN-[0-9a-f]{32}\}\}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Fact]
    public void GenerateToken_HasExpectedFormat()
    {
        var token = _tokenGenerator.GenerateToken();

        Assert.Matches(TokenPattern, token);
    }

    [Fact]
    public void GenerateToken_ProducesUniqueTokens()
    {
        var first = _tokenGenerator.GenerateToken();
        var second = _tokenGenerator.GenerateToken();

        Assert.NotEqual(first, second);
    }
}