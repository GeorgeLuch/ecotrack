using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI dos serviços
builder.Services.AddScoped<ComplianceService>();
builder.Services.AddScoped<EmissoesService>();
builder.Services.AddScoped<LicencasService>();

var app = builder.Build();

// Garante criação do banco na 1ª execução, sem falhar se já existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
    }
    catch (SqlException ex) when (ex.Number == 1801) // Database already exists
    {
        // Ignora: banco já existe
    }
}

// ✅ Swagger habilitado em TODOS os ambientes (Dev/Staging/Prod)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoTrack API v1");
    c.RoutePrefix = "swagger"; // UI em /swagger
    // Se quiser a UI na raiz do site, use:
    // c.RoutePrefix = string.Empty;
});

// Obs: em container servindo só HTTP/8080, este middleware pode logar um WARN.
// Se quiser evitar o aviso, comente a linha abaixo.
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
