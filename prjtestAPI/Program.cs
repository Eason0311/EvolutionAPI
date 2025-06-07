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

var builder = WebApplication.CreateBuilder(args);

// 1. �]�w��x
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// 2. ���U DbContext
builder.Services.AddDbContext<EvolutionApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 4. ���U UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 5. ���U JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 6. ���U�U�� Service / Helper
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IUserActionTokenService, UserActionTokenService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompOrderService, CompOrderService>();
builder.Services.AddScoped<IEmpOrderService, EmpOrderService>();
builder.Services.AddScoped<ICourseService, CourseService>();

// 7. ���U�U�� Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDepListRepository, DepListRepository>();
builder.Services.AddScoped<ICompOrderRepository, CompOrderRepository>();
builder.Services.AddScoped<IEmpOrderRepository, EmpOrderRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

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

                // 1. ���o�ثe�ШD�� Endpoint
                var endpoint = context.HttpContext.GetEndpoint();
                // 2. �q Metadata ���X [Authorize] �W�� Roles
                var authorizeAttrs = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
                string requiredRoles = "";
                if (authorizeAttrs != null && authorizeAttrs.Count > 0)
                {
                    // �i��s�b�h�� [Authorize]�A�o��X�֥��̪� Roles �ݩ�
                    requiredRoles = string.Join(",",
                        authorizeAttrs
                            .Where(a => !string.IsNullOrEmpty(a.Roles))
                            .Select(a => a.Roles));
                }

                // 3. ���o��e�ϥΪ̪����� (�p�G���h�ӡA�H�r�����j)
                var userRoles = context.HttpContext.User?.FindAll(ClaimTypes.Role)
                                   .Select(c => c.Value)
                                   .ToList()
                               ?? new List<string>();
                string currentRoles = userRoles.Count > 0
                    ? string.Join(",", userRoles)
                    : "���n�J�Ψ��⥼��";

                // 4. �եX�n�^�Ǫ� JSON ���e
                var payload = new
                {
                    success = false,
                    // �p�G endpoint �S���w Roles�A�N��ܡu�L�k�s���v���q�ΰT��
                    error = string.IsNullOrEmpty(requiredRoles)
                        ? "�z�S���s�����귽���v��"
                        : $"���\��ݭn {requiredRoles} �v���A�z�ثe�����O {currentRoles}"
                };
                var json = JsonSerializer.Serialize(payload);
                await context.Response.WriteAsync(json);
            }
        };
    });

// 9. ���v (Authorization)
builder.Services.AddAuthorization();

// 10. �[�J Controllers
builder.Services.AddControllers();

// 11. Swagger �]�w
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT ���v���Y�榡�GBearer {token}",
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

// 12. �ϥ� CORS
app.UseCors("AllowAll");

// 13. �ϥΦۭq Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStatusCodePages(); // ���ն��q��� 401, 403 �����A�X
}

// 14. HTTPS ���ɦV
app.UseHttpsRedirection();

// 15. �������� (Authentication)
app.UseAuthentication();

// 16. �ۭq JwtAuthorizationMiddleware (�Y�����n)
app.UseMiddleware<JwtAuthorizationMiddleware>();

// 17. ���v (Authorization)
app.UseAuthorization();

// 18. �ҥ��R�A�ɮתA��
app.UseStaticFiles();

// 19. Map Controllers
app.MapControllers();

app.Run();
