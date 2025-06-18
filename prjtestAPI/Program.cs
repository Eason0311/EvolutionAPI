using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Services;
using prjtestAPI;
using prjtestAPI.Helpers;
using prjtestAPI.Middleware;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using prjtestAPI.Services;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using prjEvolutionAPI.Models;
using Microsoft.AspNetCore.Http.Features;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.Account;

var builder = WebApplication.CreateBuilder(args);

// 1. 設定日誌
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// 2. 註冊 DbContext
builder.Services.AddDbContext<EvolutionApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2-1. 註冊 記憶體快取
builder.Services.AddMemoryCache();

// 註冊 SignalR
builder.Services.AddSignalR();

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:4200") // 明確指定
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // 搭配憑證傳遞（前端 withCredentials: true）
});


// 課程清除服務
builder.Services.AddHostedService<SDraftCleanupService>();

// 設定 FormOptions 以允許大檔案上傳
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10L * 1024 * 1024 * 1024; // 10GB
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10L * 1024 * 1024 * 1024; // 10GB
});

// 4. 註冊 UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 5. 註冊 JwtSettings , LinePay 相關設定的 Configure
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<LinePayOptions>(builder.Configuration.GetSection("LinePay"));

// 6. 註冊各種 Service / Helper
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<ILinePayService, LinePayService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IUserActionTokenService, UserActionTokenService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompOrderService, CompOrderService>();
builder.Services.AddScoped<IEmpOrderService, EmpOrderService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IChapterService, SChapterService>();
builder.Services.AddScoped<IVideoService, SVideoService>();
builder.Services.AddScoped<ICourseHashTagService, SCourseHashTagService>();
builder.Services.AddScoped<IDepListService, SDepListService>();
builder.Services.AddScoped<ICourseAccessService, SCourseAccessService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<ICourseAccessService, SCourseAccessService>(); 
builder.Services.AddScoped<IHashTagListService, SHashTagListService>();
builder.Services.AddScoped<ICourseBgListService, SCourseBgListService>();
// 7. 註冊各種 Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDepListRepository, DepListRepository>();
builder.Services.AddScoped<ICompOrderRepository, CompOrderRepository>();
builder.Services.AddScoped<IEmpOrderRepository, EmpOrderRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IQuizResultsRepository, QuizResultsRepository>();
builder.Services.AddScoped<IHashTagListRepository, HashTagListRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IChapterRepository, RChapterRepository>();
builder.Services.AddScoped<IVideoRepository, RVideoRepository>();
builder.Services.AddScoped<ICourseHashTagRepository, RCourseHashTagRepository>();
builder.Services.AddScoped<ICourseAccessRepository, RCourseAccessRepository>();
builder.Services.AddScoped<ICreateCourseRepository, RCreateCourseRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<ICourseBgListRepository, RCourseBgListRepository>();

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

        options.Events = new JwtBearerEvents
        {
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json; charset=utf-8";

                // 1. 取得目前請求的 Endpoint
                var endpoint = context.HttpContext.GetEndpoint();
                // 2. 從 Metadata 取出 [Authorize] 上的 Roles
                var authorizeAttrs = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
                string requiredRoles = "";
                if (authorizeAttrs != null && authorizeAttrs.Count > 0)
                {
                    // 可能存在多個 [Authorize]，這邊合併它們的 Roles 屬性
                    requiredRoles = string.Join(",",
                        authorizeAttrs
                            .Where(a => !string.IsNullOrEmpty(a.Roles))
                            .Select(a => a.Roles));
                }

                // 3. 取得當前使用者的角色 (如果有多個，以逗號分隔)
                var userRoles = context.HttpContext.User?.FindAll(ClaimTypes.Role)
                                   .Select(c => c.Value)
                                   .ToList()
                               ?? new List<string>();
                string currentRoles = userRoles.Count > 0
                    ? string.Join(",", userRoles)
                    : "未登入或角色未知";

                // 4. 組出要回傳的 JSON 內容
                var payload = new
                {
                    success = false,
                    // 如果 endpoint 沒指定 Roles，就顯示「無法存取」的通用訊息
                    error = string.IsNullOrEmpty(requiredRoles)
                        ? "您沒有存取此資源的權限"
                        : $"此功能需要 {requiredRoles} 權限，您目前身分是 {currentRoles}"
                };
                var json = JsonSerializer.Serialize(payload);
                await context.Response.WriteAsync(json);
            }
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
app.UseCors("AllowFrontend");

//SignalR套件
//Install-Package Microsoft.AspNetCore.SignalR
app.MapHub<CourseHub>("/courseHub");

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
