namespace BootcampCLT2026.Domain
{
    public interface ICuentaRepository
    {
        Task<IEnumerable<Cuenta>?> ObtenerCuentasAsync(CancellationToken cancellationToken = default);
        Task<Cuenta?> ObtenerCuentaPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CrearCuentaAsync(Cuenta cuenta, CancellationToken cancellationToken = default);
        void ModificarCuenta(Cuenta cuenta);
        void EliminarCuenta(Cuenta cuenta);
        Task <bool> GuardarCambiosAsync(CancellationToken cancellationToken = default);
    }
}
