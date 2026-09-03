using Microsoft.EntityFrameworkCore;
using ResmapApi.Models;
using ResmapApi.Endpoints;
using ResmapApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONEXIÓN A LA BASE DE DATOS
// ==========================================

builder.Services.AddDbContext<ResmapdbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("miconexion")
    )
);

// ==========================================
// REPOSITORIES
// ==========================================

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// ==========================================
// OPENAPI
// ==========================================

builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// OPENAPI
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ==========================================
// HTTPS
// ==========================================

app.UseHttpsRedirection();

// ==========================================
// ENDPOINT PRINCIPAL
// ==========================================

app.MapGet("/", () =>
{
    return Results.Ok("ResmapApi funcionando correctamente");
});

// ==========================================
// CRUD DE PRODUCTOS
// ==========================================

app.MapProductoApi();

app.Run();