using FluentValidation;

namespace BootcampCLT2026.Application.Cuentas.Commands.EliminarCuenta
{
    public class EliminarCuentaCommandValidator : AbstractValidator<EliminarCuentaCommand>
    {
        public EliminarCuentaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id de la cuenta es obligatorio.")
                .Must(id => id != Guid.Empty).WithMessage("El Id de la cuenta no puede ser un Guid vacío.");
        }
    }
}
