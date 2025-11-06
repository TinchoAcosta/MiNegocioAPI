namespace MiNegocioAPI.model
{
    public class Pago
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public decimal monto { get; set; }
        public string metodoDePago { get; set; }
        public int turnoId { get; set; }
        public Turno turno { get; set; }

        public Pago() { }
        public Pago(int id, DateTime fecha, decimal monto, string metodoDePago, int turnoId) {
            this.id = id;
            this.fecha = fecha;
            this.monto = monto;
            this.metodoDePago = metodoDePago;
            this.turnoId = turnoId;
        }
    }
}