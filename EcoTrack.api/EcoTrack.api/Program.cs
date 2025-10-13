using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Obs: em container servindo só http/8080, este middleware pode logar um WARN.
// Se quiser, comente a linha abaixo para não ver o aviso.
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
