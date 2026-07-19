using MediatR;
namespace BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentaPorId
{
    public record ObtenerCuentaPorIdQuery(Guid Id) : IRequest<CuentaDto?>;
}
