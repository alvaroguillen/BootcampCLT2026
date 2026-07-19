using BootcampCLT2026.Domain;
using BootcampCLT2026.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BootcampCLT2026.Infraestructure.Repositories
{
    public class CuentaRepository(AppDbContext context) : ICuentaRepository
    {
        public async Task<IEnumerable<Cuenta>> ObtenerCuentasAsync(CancellationToken cancellationToken = default) =>
            await context.Cuentas.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<Cuenta?> ObtenerCuentaPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await context.Cuentas.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task CrearCuentaAsync(Cuenta cuenta, CancellationToken cancellationToken = default) =>
            await context.Cuentas.AddAsync(cuenta, cancellationToken);

        public void ModificarCuenta(Cuenta cuenta) =>
            context.Cuentas.Update(cuenta);

        public void EliminarCuenta(Cuenta cuenta) =>
            context.Cuentas.Remove(cuenta);

        public async Task<bool> GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
             await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
