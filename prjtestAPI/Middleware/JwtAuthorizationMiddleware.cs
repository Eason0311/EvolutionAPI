using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using prjtestAPI.Attributes;
using prjtestAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace prjtestAPI.Middleware
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthorizationMiddleware> _logger;
        private readonly JwtSettings _jwtSettings;

        // 路徑排除驗證（如登入、註冊、Swagger）
        private readonly string[] _excludedPaths = new[]
        {
            "/api/Auth/login",
            "/api/Auth/register",
            "/api/Auth/logout",
            "/api/Auth/refresh",
            "/swagger",
            "/favicon.ico"
        };

        public JwtAuthorizationMiddleware(RequestDelegate next, ILogger<JwtAuthorizationMiddleware> logger, IOptions<JwtSettings> jwtOptions)
        {
            _next = next;
            _logger = logger;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>();

            // ✅ 若是 AllowAnonymous 或根本沒有 endpoint（例如 /favicon.ico），就放行
            if (endpoint == null || allowAnonymous != null)
            {
                await _next(context);
                return;
            }

            if (_excludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var token = ExtractTokenFromHeader(context);
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("缺少 Bearer Token");
                await ReturnUnauthorized(context, "未提供 Bearer Token");
                return;
            }

            if (!TryValidateToken(token, out var principal))
            {
                _logger.LogWarning("JWT 驗證失敗，拒絕訪問");
                await ReturnUnauthorized(context, "無效的 Token");
                return;
            }

            // 要先設定 context.User，後面授權才能成功判斷
            context.User = principal;

            // 讀取 Controller 或 Action 上的 RequiredRoleAttribute
            var requiredRoles = context.GetEndpoint()?.Metadata.GetMetadata<RequiredRoleAttribute>()?.Roles;

            if (requiredRoles?.Length > 0)
            {
                if (!requiredRoles.Any(role => context.User.IsInRole(role)))
                {
                    var roleList = string.Join(", ", requiredRoles);
                    _logger.LogWarning("拒絕訪問：使用者缺乏角色，需為其中一種: {roles}", roleList);
                    await ReturnUnauthorized(context, $"需要角色: {roleList}");
                    return;
                }
            }

            await _next(context);
        }

        private string? ExtractTokenFromHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader["Bearer ".Length..].Trim();
            }
            return null;
        }

        private bool TryValidateToken(string token, out ClaimsPrincipal principal)
        {
            principal = null!;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token 驗證錯誤: {Message}", ex.Message);
                return false;
            }
        }

        private async Task ReturnUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ApiResponse<string>.FailResponse(message, null, 401));
        }
    }
}
