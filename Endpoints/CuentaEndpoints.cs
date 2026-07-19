using BootcampCLT2026.Application.Cuentas;
using BootcampCLT2026.Application.Cuentas.Commands.CrearCuenta;
using BootcampCLT2026.Application.Cuentas.Commands.EliminarCuenta;
using BootcampCLT2026.Application.Cuentas.Commands.ModificarCuenta;
using BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentaPorId;
using BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentas;
using BootcampCLT2026.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace BootcampCLT2026.Endpoints
{
    public static class CuentaEndpoints
    {
        public static void MapCuentaEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("v1/api/cuenta").WithTags("Cuenta (Minimal API)");

            group.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var cuentas = await sender.Send(new ObtenerCuentasQuery(), cancellationToken);
                return cuentas is null ? Results.NotFound() : Results.Ok(cuentas);
            })
                .WithName("ObtenerCuentas")
                .Produces<IEnumerable<CuentaDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            group.MapGet("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                if (id == Guid.Empty)
                    return Results.BadRequest("El id es inválido.");

                var cuenta = await sender.Send(new ObtenerCuentaPorIdQuery(id), cancellationToken);

                return cuenta is null ? Results.NotFound() : Results.Ok(cuenta);
            })
                .WithName("ObtenerCuentaPorId")
                .Produces<CuentaDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError); 

            group.MapPost("", async(CrearCuentaCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var cuenta = await sender.Send(command, cancellationToken);
                return Results.Created($"v1/api/cuenta/{cuenta.Id}", cuenta); 
            })
                .WithName("CrearCuenta")
                .Produces<CuentaDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            group.MapPut("{id:guid}", async (Guid id, ModificarCuentaBody body, ISender sender, CancellationToken cancellationToken) =>
            {
                var cuenta = await sender.Send(new ModificarCuentaCommand(id, body.NumeroCuenta, body.NombreTitular, body.Saldo, body.Estado), cancellationToken);
                return cuenta is null ? Results.NotFound() : Results.Ok(cuenta);
            })
                .WithName("ModificarCuenta")
                .Produces<CuentaDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            group.MapDelete("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var eliminar = await sender.Send(new EliminarCuentaCommand(id), cancellationToken);
                return eliminar ? Results.NoContent() : Results.NotFound();
            })
                .WithName("EliminarCuenta")
                .Produces<CuentaDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
        }

        public record ModificarCuentaBody(string NumeroCuenta, string NombreTitular, decimal Saldo, string Estado);
    }
}