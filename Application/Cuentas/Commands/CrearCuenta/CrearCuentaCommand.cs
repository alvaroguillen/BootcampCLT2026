using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.CrearCuenta
{
    public record CrearCuentaCommand(string NumeroCuenta, string NombreTitular, decimal Saldo, string Estado) : IRequest<CuentaDto>;
}
