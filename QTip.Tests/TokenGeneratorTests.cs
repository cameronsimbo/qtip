using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using QTip.Application.Abstractions;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public class TokenGeneratorTests : TestBase
{
    private readonly ITokenGenerator _tokenGenerator;

    public TokenGeneratorTests()
    {
        ServiceProvider provider = CreateServiceProvider();
        _tokenGenerator = provider.GetRequiredService<ITokenGenerator>();
    }

    private static readonly Regex TokenPattern = new(
        pattern: @"^\{\{TKN-[0-9a-f]{32}\}\}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Fact]
    public void GenerateToken_HasExpectedFormat()
    {
        string token = _tokenGenerator.GenerateToken();

        Assert.Matches(TokenPattern, token);
    }

    [Fact]
    public void GenerateToken_ProducesUniqueTokens()
    {
        string first = _tokenGenerator.GenerateToken();
        string second = _tokenGenerator.GenerateToken();

        Assert.NotEqual(first, second);
    }
}

