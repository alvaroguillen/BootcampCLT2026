using BootcampCLT2026.Domain;
using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.EliminarCuenta
{
    public class EliminarCuentaCommandHandler(ICuentaRepository cuentaRepository, ILogger<EliminarCuentaCommandHandler> logger)
        : IRequestHandler<EliminarCuentaCommand, bool>
    {
        public async Task<bool> Handle(EliminarCuentaCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Buscando cuenta con Id {request.Id} para eliminar");

            var cuenta = await cuentaRepository.ObtenerCuentaPorIdAsync(request.Id, cancellationToken);
            if (cuenta == null)
            {
                logger.LogWarning($"No se encontró la cuenta con Id {request.Id}. No se pudo eliminar", request.Id);
                return false;
            }

            cuentaRepository.EliminarCuenta(cuenta);
            await cuentaRepository.GuardarCambiosAsync(cancellationToken);

            logger.LogInformation($"Cuenta con Id {request.Id} eliminada exitosamente");

            return true;
        }
    }
}