using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using prjtestAPI.Helpers;
using prjtestAPI.Models;
using prjtestAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public int RefreshTokenExpirationDays => _jwtSettings.RefreshTokenExpirationDays;

    public JwtService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateAccessToken(TUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<TRefreshToken> GenerateRefreshToken(int userId, string? ipAddress, string? userAgent)
    {
        var token = Guid.NewGuid().ToString("N") + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var refreshToken = new TRefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IpAddress = ipAddress ?? string.Empty,
            UserAgent = userAgent ?? string.Empty,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(refreshToken);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(60),
                RoleClaimType = ClaimTypes.Role
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}