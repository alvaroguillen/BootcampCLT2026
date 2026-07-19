using MediatR;

namespace BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentas
{
    public record ObtenerCuentasQuery : IRequest<IEnumerable<CuentaDto>?>;
}
