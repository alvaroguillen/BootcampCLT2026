using BootcampCLT2026.Domain;
using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentaPorId
{
    public class ObtenerCuentaPorIdQueryHandler(ICuentaRepository repository, ILogger<ObtenerCuentaPorIdQueryHandler> logger)
        : IRequestHandler<ObtenerCuentaPorIdQuery, CuentaDto?>
    {
        public async Task<CuentaDto?> Handle(ObtenerCuentaPorIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Consultando cuenta con Id {request.Id}");

            var cuenta = await repository.ObtenerCuentaPorIdAsync(request.Id, cancellationToken);
            if (cuenta is null)
            {
                logger.LogWarning($"No se encontró la cuenta con Id {request.Id}", request.Id);
                return null;
            }

            return new CuentaDto(cuenta.Id, cuenta.NumeroCuenta, cuenta.NombreTitular, cuenta.Saldo, cuenta.Estado, cuenta.FechaCreacion);
        }
    }
}