using BootcampCLT2026.Domain;

namespace BootcampCLT2026.Application.Cuentas
{
    public static class CuentaMappingExtensions
    {
        public static CuentaDto cuentaDto(this Cuenta cuenta) => 
            new (cuenta.Id, cuenta.NumeroCuenta, cuenta.NombreTitular, cuenta.Saldo, cuenta.Estado, cuenta.FechaCreacion);
    }
}
