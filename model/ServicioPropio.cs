namespace MiNegocioAPI.model
{
    public class ServicioPropio
    {
        public int id { get; set; }
        public int duracionMinutos { get; set; }
        public decimal precioBase { get; set; }
        public int servicioId { get; set; }
        public int usuarioId { get; set; }
        public Usuario usuario { get; set; }
        public ServicioBase servicioBase { get; set; }
        public ICollection<Promo> promos { get; set; } = new List<Promo>();
        public ICollection<Turno> turnos { get; set; } = new List<Turno>();

        public ServicioPropio() { }
        public ServicioPropio(int id, int duracionMinutos, decimal precioBase, int servicioId, int usuarioId)
        {
            this.id = id;
            this.duracionMinutos = duracionMinutos;
            this.precioBase = precioBase;
            this.servicioId = servicioId;
            this.usuarioId = usuarioId;
        }
    }
}