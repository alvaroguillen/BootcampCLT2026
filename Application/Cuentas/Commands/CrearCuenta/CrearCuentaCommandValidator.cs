using FluentValidation;

namespace BootcampCLT2026.Application.Cuentas.Commands.CrearCuenta
{
    public class CrearCuentaCommandValidator : AbstractValidator<CrearCuentaCommand>
    {
        public CrearCuentaCommandValidator()
        {
            RuleFor(x => x.NumeroCuenta)
                .NotEmpty().WithMessage("El número de cuenta es obligatorio.")
                .Length(10).WithMessage("El número de cuenta debe tener 10 dígitos.");
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
