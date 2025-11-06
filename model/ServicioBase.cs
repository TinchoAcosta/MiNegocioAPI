namespace MiNegocioAPI.model
{
    public class ServicioBase
    {
        public int id { get; set; }
        public string detalle { get; set; }
        public string categoria { get; set; }

        public ServicioBase() { }
        public ServicioBase(int id, string detalle, string categoria)
        {
            this.id = id;
            this.detalle = detalle;
            this.categoria = categoria;
        }
    }
}