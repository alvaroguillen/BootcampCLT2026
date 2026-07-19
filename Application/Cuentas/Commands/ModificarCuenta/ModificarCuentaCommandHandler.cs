using BootcampCLT2026.Domain;
using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.ModificarCuenta
{
    public class ModificarCuentaCommandHandler(ICuentaRepository cuentaRepository, ILogger<ModificarCuentaCommandHandler> logger)
        : IRequestHandler<ModificarCuentaCommand, CuentaDto>
    {
        public async Task<CuentaDto> Handle(ModificarCuentaCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Buscando cuenta con Id {request.Id} para modificar");

            var cuenta = await cuentaRepository.ObtenerCuentaPorIdAsync(request.Id, cancellationToken);
            if (cuenta == null)
            {
                logger.LogWarning($"No se encontró la cuenta con Id {request.Id}", request.Id);
                throw new Exception($"La cuenta con Id {request.Id} no existe.");
            }

            cuenta.NumeroCuenta = request.NumeroCuenta;
            cuenta.NombreTitular = request.NombreTitular;
            cuenta.Saldo = request.Saldo;
            cuenta.Estado = request.Estado;

            cuentaRepository.ModificarCuenta(cuenta);
            await cuentaRepository.GuardarCambiosAsync(cancellationToken);

            logger.LogInformation($"Cuenta con Id {cuenta.Id} modificada exitosamente", cuenta.Id);

            return cuenta.cuentaDto();
        }
    }
}