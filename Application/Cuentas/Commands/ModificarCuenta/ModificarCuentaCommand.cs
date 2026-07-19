using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.ModificarCuenta
{
    public record ModificarCuentaCommand(
        Guid Id,
        string NumeroCuenta,
        string NombreTitular,
        decimal Saldo,
        string Estado) : IRequest<CuentaDto>;
}
