using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Services;
using prjtestAPI;
using prjtestAPI.Data;
using prjtestAPI.Helpers;
using prjtestAPI.Middleware;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using prjtestAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. 設定日誌
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// 2. 註冊 DbContext
builder.Services.AddDbContext<TestApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 4. 註冊 UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 5. 註冊 JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 6. 註冊各種 Service / Helper
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IUserActionTokenService, UserActionTokenService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();

// 7. 註冊各種 Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// 8. JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        var secretKey = config["JwtSettings:SecretKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });

// 9. 授權 (Authorization)
builder.Services.AddAuthorization();

// 10. 加入 Controllers
builder.Services.AddControllers();

// 11. Swagger 設定
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT 授權標頭格式：Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 12. 使用 CORS
app.UseCors("AllowAll");

// 13. 使用自訂 Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStatusCodePages(); // 測試階段顯示 401, 403 等狀態碼
}

// 14. HTTPS 重導向
app.UseHttpsRedirection();

// 15. 身份驗證 (Authentication)
app.UseAuthentication();

// 16. 自訂 JwtAuthorizationMiddleware (若有必要)
app.UseMiddleware<JwtAuthorizationMiddleware>();

// 17. 授權 (Authorization)
app.UseAuthorization();

// 18. 啟用靜態檔案服務
app.UseStaticFiles();

// 19. Map Controllers
app.MapControllers();

app.Run();
