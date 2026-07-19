namespace BootcampCLT2026.Application.Cuentas
{
    public record CuentaDto(
     Guid Id,
    string NumeroCuenta,
    string NombreTitular,
    decimal Saldo,
    string Estado,
    DateTime FechaCreacion);
}