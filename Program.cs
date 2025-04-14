using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AnimeNowApi.Services;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger 配置
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AnimeNow API", Version = "v1" });
});

builder.Services.AddDbContext<AnimeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(Program));

// 添加 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// 添加認證服務
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        builder.Configuration["TokenSettings:Key"] ?? "DefaultSecureKeyForDevelopment1234!")),
            ValidIssuer = builder.Configuration["TokenSettings:Issuer"] ?? "DefaultIssuer",
            ValidAudience = builder.Configuration["TokenSettings:Audience"] ?? "DefaultAudience",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 添加授權服務
builder.Services.AddAuthorization();

// 註冊 TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHostedService<AnimeScrapingService>();

// 註冊通知服務
builder.Services.AddScoped<INotificationService, NotificationService>();

// 建立應用程序
var app = builder.Build();

// 無條件啟用 Swagger 
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnimeNow API v1"));

app.UseCors("AllowVueApp");

// 認證和授權
app.UseAuthentication();
app.UseAuthorization();

// 控制器對映
app.MapControllers();

// 初始化記憶體資料庫
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AnimeDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "發生錯誤，無法初始化資料。");
    }
}

app.UseStaticFiles();
app.Run();