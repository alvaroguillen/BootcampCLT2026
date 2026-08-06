using FluentValidation;
using System.Diagnostics;

namespace BootcampCLT2026.Middleware
{
    public static class ValidationExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidationExceptionHandling(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Api-Cuenta.Middleware.ExceptionHandling");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
                stopwatch.Stop();

                // Forma correcta de loguear: placeholders con nombre (structured logging),
                // nunca interpolación de strings ($"..."). Así Seq puede indexar y filtrar
                // por cada propiedad (Method, Path, StatusCode, etc.) en vez de tratar todo
                // el mensaje como texto plano.
                logger.LogInformation(
                    "HTTP {Method} {Path} respondió {StatusCode} en {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (ValidationException ex)
            {
                stopwatch.Stop();

                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                // Una regla de negocio/entrada inválida no es un error del servidor:
                // se registra como Warning, no como Error.
                logger.LogWarning(
                    "Validación fallida para {Method} {Path}: {ErrorCount} error(es) -> {@Errors}",
                    context.Request.Method,
                    context.Request.Path,
                    errors.Count,
                    errors);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    title = "Validation error",
                    status = StatusCodes.Status400BadRequest,
                    errors
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Excepción no controlada -> Error, con la excepción completa para el stack trace.
                logger.LogError(
                    ex,
                    "Excepción no controlada procesando {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    title = "An unexpected error occurred",
                    status = StatusCodes.Status500InternalServerError
                });
            }
        }); 
    }
}
