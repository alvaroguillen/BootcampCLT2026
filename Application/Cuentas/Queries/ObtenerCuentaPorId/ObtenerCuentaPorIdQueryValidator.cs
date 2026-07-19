using FluentValidation;

namespace BootcampCLT2026.Application.Cuentas.Queries.ObtenerCuentaPorId
{
    public class ObtenerCuentaPorIdQueryValidator : AbstractValidator<ObtenerCuentaPorIdQuery>
    {
        public ObtenerCuentaPorIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("El Id de la cuenta no puede estar vacío.");
        }
    }
}
