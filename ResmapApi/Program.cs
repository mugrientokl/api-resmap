using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResmapApi.Endpoints;
using ResmapApi.Middleware;
using ResmapApi.Models;
using ResmapApi.Repositories;
using ResmapApi.Services;
using Scalar.AspNetCore;
using System.Text;
using ResmapApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "No se encontró la clave JWT en la configuración.");
}

builder.Services.AddDbContext<ResmapdbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("miconexion")
    )
);

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddScoped<
    ISolicitudPedidoRepository,
    SolicitudPedidoRepository>();

builder.Services.AddScoped<
    ISolicitudProveedorRepository,
    SolicitudProveedorRepository>();


builder.Services.AddScoped<AuthService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
    {
        policy.RequireRole("Administrador");
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        mensaje = "ResmapApi funcionando correctamente",
        version = "v1"
    });
});

app.MapProductoApi();
app.MapAuthApi();
app.MapSolicitudPedidoApi();
app.MapSolicitudProveedorApi();

app.Run();