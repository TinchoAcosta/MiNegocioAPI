public class TurnoDTO
{
      public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Descripcion { get; set; }
    public string? Estado { get; set; }

    public string? ClienteNombre { get; set; }
    public string? ServicioNombre { get; set; }
    public string? Categoria { get; set; }
    public decimal PrecioBase { get; set; }
    public string? PromoDescripcion { get; set; }
    public decimal? PrecioPromo { get; set; }
    public int? clienteId { get; set; }

    public List<PagoDTO>? Pagos { get; set; }
}
