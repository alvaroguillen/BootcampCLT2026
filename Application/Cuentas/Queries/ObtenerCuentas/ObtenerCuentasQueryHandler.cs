using BootcampCLT2026.Domain;
using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentas
{
    public class ObtenerCuentasQueryHandler(ICuentaRepository cuentaRepository, ILogger<ObtenerCuentasQueryHandler> logger)
        : IRequestHandler<ObtenerCuentasQuery, IEnumerable<CuentaDto>?>
    {
        public async Task<IEnumerable<CuentaDto>?> Handle(ObtenerCuentasQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Consultando listado de cuentas");

            var cuentas = await cuentaRepository.ObtenerCuentasAsync(cancellationToken);
            if (cuentas is null)
            {
                logger.LogWarning("No se encontraron cuentas");
                return null;
            }

            var resultado = cuentas.Select(cuenta => cuenta.cuentaDto()).ToList();

            logger.LogInformation($"Se encontraron {resultado.Count} cuentas");

            return resultado;
        }
    }
}