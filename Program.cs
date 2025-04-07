using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Mappings;

var builder = WebApplication.CreateBuilder(args);

// 添加服務到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AnimeNow API", Version = "v1" });
});

// 使用 InMemory 資料庫而非 SQL Server
builder.Services.AddDbContext<AnimeDbContext>(options =>
    options.UseInMemoryDatabase("AnimeDb"));

// 添加 AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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

// 建立應用程序
var app = builder.Build();

// 無條件啟用 Swagger 
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnimeNow API v1"));


//app.UseHttpsRedirection();

app.UseCors("AllowVueApp");
app.UseAuthorization();
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

app.Run();