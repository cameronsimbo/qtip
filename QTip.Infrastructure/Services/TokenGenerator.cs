using QTip.Application.Abstractions;

namespace QTip.Infrastructure.Services;

public sealed class TokenGenerator : ITokenGenerator
{
    public string GenerateToken()
    {
        return $"{{{{TKN-{Guid.NewGuid():N}}}}}";
    }
}


