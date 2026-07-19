namespace BootcampCLT2026.Domain
{
    public class Cuenta
    {
        public Guid Id { get; set; }
        public string NumeroCuenta { get; set; }

        private string _nombreTitular;
        public string NombreTitular
        {
            get => _nombreTitular;
            set => _nombreTitular = value?.ToUpperInvariant();
        }

        public decimal Saldo { get; set; }

        private string _estado;
        public string Estado
        {
            get => _estado;
            set => _estado = value?.ToUpperInvariant();
        }

        public DateTime FechaCreacion { get; set; }
    }
}