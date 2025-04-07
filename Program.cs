using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AnimeNowApi.Data;
using AnimeNowApi.Mappings;

var builder = WebApplication.CreateBuilder(args);

// �K�[�A�Ȩ�e��
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AnimeNow API", Version = "v1" });
});

// �ϥ� InMemory ��Ʈw�ӫD SQL Server
builder.Services.AddDbContext<AnimeDbContext>(options =>
    options.UseInMemoryDatabase("AnimeDb"));

// �K�[ AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// �K�[ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// �إ����ε{��
var app = builder.Build();

// �L����ҥ� Swagger 
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnimeNow API v1"));


//app.UseHttpsRedirection();

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

// ��l�ưO�����Ʈw
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
        logger.LogError(ex, "�o�Ϳ��~�A�L�k��l�Ƹ�ơC");
    }
}

app.Run();