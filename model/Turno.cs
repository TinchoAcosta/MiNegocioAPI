namespace MiNegocioAPI.model
{
    public class Turno
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public EstadoTurno estado { get; set; }
        public int clienteId { get; set; }
        public int servicioId { get; set; }
        public int promoId { get; set; }
        public Cliente cliente { get; set; }
        public ServicioPropio servicio { get; set; }
        public Promo promo { get; set; }
        public ICollection<Pago> pagos { get; set; }

        public Turno() { }
        public Turno(int id, DateTime fecha, string descripcion, EstadoTurno estado, int clienteId, int servicioId, int promoId)
        {
            this.id = id;
            this.fecha = fecha;
            this.descripcion = descripcion;
            this.estado = estado;
            this.clienteId = clienteId;
            this.servicioId = servicioId;
            this.promoId = promoId;
        }
    }
}