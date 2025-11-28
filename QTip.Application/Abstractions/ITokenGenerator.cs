namespace QTip.Application.Abstractions;

public interface ITokenGenerator
{
    /// <summary>
    /// Generates a new opaque token value suitable for replacing sensitive data.
    /// </summary>
    /// <returns>A unique token string.</returns>
    string GenerateToken();
}


