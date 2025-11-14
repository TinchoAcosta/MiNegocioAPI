public class HistorialPagoDTO
{
    public int Id { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string MetodoDePago { get; set; }

    // Cliente
    public string ClienteNombre { get; set; }
    public string ClienteApellido { get; set; }

    // Turno
    public DateTime FechaTurno { get; set; }

    // Servicio
    public string ServicioDetalle { get; set; }

    // Promo opcional
    public string PromoDescripcion { get; set; }
}
