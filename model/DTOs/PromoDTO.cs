public class PromoDTO
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public string? Imagen { get; set; }
    public decimal? PrecioNuevo { get; set; }
    public string? Condicion { get; set; }
    public DateTime FechaFin { get; set; }

    public ServicioPropioDTO ServicioPropio { get; set; }

    
}
