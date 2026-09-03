using Microsoft.EntityFrameworkCore;
using ResmapApi.Models;
using ResmapApi.Endpoints;
using ResmapApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ResmapdbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("miconexion")
    )
);

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return Results.Ok("ResmapApi funcionando correctamente");
});

app.MapProductoApi();

app.Run();