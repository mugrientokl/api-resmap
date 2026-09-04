using System.Text.Json;

namespace ResmapApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error no controlado en la API.");

                context.Response.StatusCode = 500;
                context.Response.ContentType =
                    "application/json";

                var respuesta = new
                {
                    mensaje =
                        "Ocurrió un error interno en el servidor.",
                    codigo = 500
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(respuesta));
            }
        }
    }
}