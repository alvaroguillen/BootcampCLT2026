using FluentValidation;

namespace BootcampCLT2026.Application.Cuentas.Commands.ModificarCuenta
{
    public class ModificarCuentaCommandValidator : AbstractValidator<ModificarCuentaCommand>
    {
        public ModificarCuentaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id de la cuenta es obligatorio.")
                .Must(id => id != Guid.Empty).WithMessage("El Id de la cuenta no puede ser un Guid vacío.");
            RuleFor(x => x.NumeroCuenta)
                .NotEmpty().WithMessage("El número de cuenta es obligatorio.")
                .MaximumLength(20).WithMessage("El número de cuenta no puede exceder los 20 caracteres.");
            RuleFor(x => x.NombreTitular)
                .NotEmpty().WithMessage("El nombre del titular es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del titular no puede exceder los 100 caracteres.");
            RuleFor(x => x.Saldo)
                .GreaterThanOrEqualTo(0).WithMessage("El saldo no puede ser negativo.");
            RuleFor(x => x.Estado)
                .Must(estado => estado is not null &&
                                (estado.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase) ||
                                 estado.Equals("INACTIVO", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("El estado de la cuenta debe ser ACTIVO o INACTIVO.");
        }
    }
}
