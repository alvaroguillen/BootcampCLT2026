using BootcampCLT2026.Domain;
using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.CrearCuenta
{
    public class CrearCuentaCommandHandler(ICuentaRepository cuentaRepository, ILogger<CrearCuentaCommandHandler> logger)
        : IRequestHandler<CrearCuentaCommand, CuentaDto>
    {
        public async Task<CuentaDto> Handle(CrearCuentaCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Creando cuenta con número {request.NumeroCuenta} para {request.NombreTitular}");

            var cuenta = new Cuenta
            {
                Id = Guid.NewGuid(),
                NumeroCuenta = request.NumeroCuenta,
                NombreTitular = request.NombreTitular,
                Saldo = request.Saldo,
                Estado = request.Estado,
                FechaCreacion = DateTime.UtcNow
            };

            await cuentaRepository.CrearCuentaAsync(cuenta, cancellationToken);
            await cuentaRepository.GuardarCambiosAsync(cancellationToken);

            logger.LogInformation($"Cuenta creada exitosamente con Id {cuenta.Id}");

            return cuenta.cuentaDto();
        }
    }
}