using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Commands.EliminarCuenta
{
    public record EliminarCuentaCommand(Guid Id) : IRequest<bool>;
}
