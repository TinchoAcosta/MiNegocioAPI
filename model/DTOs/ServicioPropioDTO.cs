public class ServicioPropioDTO
{
    public int Id { get; set; } // id del servicio propio
    public decimal PrecioBase { get; set; }
    public int DuracionMinutos { get; set; }
    public string Detalle { get; set; } // del ServicioBase
    public string Categoria { get; set; } // del ServicioBase

    public ServicioPropioDTO() { }

    public ServicioPropioDTO(int id, decimal precioBase, int duracionMinutos, string detalle, string categoria)
    {
        Id = id;
        PrecioBase = precioBase;
        DuracionMinutos = duracionMinutos;
        Detalle = detalle;
        Categoria = categoria;
    }
}
