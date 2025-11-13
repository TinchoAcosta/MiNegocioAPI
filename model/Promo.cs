namespace MiNegocioAPI.model
{
    public class Promo
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public DateTime fechaFin { get; set; }
        public string? imagen { get; set; }
        public decimal? precioNuevo { get; set; }
        public string? condicion { get; set; }
        public int servicioPropioId { get; set; }
        public ServicioPropio servicioPropio { get; set; }
        public ICollection<Turno> turnos { get; set; } = new List<Turno>();
        public bool estado { get; set; }

        public Promo() { }
        public Promo(int id, string descripcion, DateTime fechaFin, string? imagen, decimal? precioNuevo, string? condicion, int servicioPropioId) {
            this.id = id;
            this.descripcion = descripcion;
            this.fechaFin = fechaFin;
            this.imagen = imagen;
            this.precioNuevo = precioNuevo;
            this.condicion = condicion;
            this.servicioPropioId = servicioPropioId;
        } 
    }
}