using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResmapApi.DTOs;
using ResmapApi.Models;
using ResmapApi.Services;

namespace ResmapApi.Endpoints
{
    public static class AuthApi
    {
        public static void MapAuthApi(this WebApplication app)
        {
            var grupo = app
                .MapGroup("/api/auth")
                .WithTags("Autenticación");

            // REGISTRO
            grupo.MapPost("/registro", async (
                RegistroDto registro,
                ResmapdbContext db,
                AuthService authService) =>
            {
                if (string.IsNullOrWhiteSpace(registro.Nombre))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El nombre es obligatorio"
                    });
                }

                if (string.IsNullOrWhiteSpace(registro.Email))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El email es obligatorio"
                    });
                }

                if (string.IsNullOrWhiteSpace(registro.Password))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "La contraseña es obligatoria"
                    });
                }

                var emailExiste = await db.Usuarios
                    .AnyAsync(u => u.Email == registro.Email);

                if (emailExiste)
                {
                    return Results.Conflict(new
                    {
                        mensaje = "Ya existe un usuario con ese email"
                    });
                }

                var usuario = new Usuario
                {
                    Nombre = registro.Nombre,
                    Email = registro.Email,
                    Rut = registro.Rut,

                    // Todo usuario que se registre por esta ruta
                    // será Cliente.
                    RolId = 2
                };

                usuario.PasswordHash =
                    authService.HashPassword(
                        usuario,
                        registro.Password);

                db.Usuarios.Add(usuario);

                await db.SaveChangesAsync();

                return Results.Created(
                    $"/api/auth/usuario/{usuario.Id}",
                    new
                    {
                        mensaje = "Usuario registrado correctamente",
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.RolId
                    });
            });

            // LOGIN
            grupo.MapPost("/login", async (
                LoginDto login,
                ResmapdbContext db,
                AuthService authService,
                IConfiguration configuration) =>
            {
                var usuario = await db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(
                        u => u.Email == login.Email);

                if (usuario == null)
                {
                    return Results.Unauthorized();
                }

                var resultado = authService.VerifyPassword(
                    usuario,
                    login.Password);

                if (resultado != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                {
                    return Results.Unauthorized();
                }

                var key = configuration["Jwt:Key"];
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];

                if (string.IsNullOrWhiteSpace(key))
                {
                    return Results.Problem(
                        "La clave JWT no está configurada.");
                }

                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        usuario.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        usuario.Nombre),

                    new Claim(
                        ClaimTypes.Email,
                        usuario.Email),

                    new Claim(
                        ClaimTypes.Role,
                        usuario.Rol.Nombre)
                };

                var securityKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key));

                var credentials =
                    new SigningCredentials(
                        securityKey,
                        SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials);

                var tokenString =
                    new JwtSecurityTokenHandler()
                        .WriteToken(token);

                return Results.Ok(new
                {
                    mensaje = "Login correcto",
                    token = tokenString,
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        Rol = usuario.Rol.Nombre
                    }
                });
            });
        }
    }
}