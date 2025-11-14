using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MiNegocioAPI.model;

namespace MiNegocioAPI.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public PagoController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.config = config;
            environment = env;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> registrarPago([FromBody] Pago pago)
        {
            if (pago == null)
                return BadRequest("Datos inválidos.");

            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");
            int usuarioId = int.Parse(idClaim);


            var turno = await contexto.Turno
        .Include(t => t.servicio)
        .FirstOrDefaultAsync(t => t.id == pago.turnoId);

            if (turno == null)
                return NotFound("El turno no existe.");
            if (turno.servicio.usuarioId != usuarioId)
                return Unauthorized("No tiene permisos para registrar un pago en este turno.");

            bool yaTienePago = await contexto.Pago.AnyAsync(p => p.turnoId == pago.turnoId);
            if (yaTienePago)
                return BadRequest("Este turno ya tiene un pago registrado.");

            pago.turnoId = turno.id;
            pago.turno = turno;

            contexto.Pago.Add(pago);
            await contexto.SaveChangesAsync();

            return Ok(new { mensaje = "Pago registrado exitosamente." });
        }

        [HttpGet("historial")]
        public async Task<IActionResult> Get(
    [FromQuery] DateTime fechaMin,
    [FromQuery] DateTime fechaMax,
    [FromQuery] bool efectivo,
    [FromQuery] bool tarjeta,
    [FromQuery] bool transfer)
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            var query = contexto.Pago
                .Include(p => p.turno)
                    .ThenInclude(t => t.servicio)
                .Where(p =>
                    p.turno.servicio.usuarioId == usuarioId &&
                    p.fecha >= fechaMin &&
                    p.fecha <= fechaMax
                );

            var metodos = new List<string>();
            if (efectivo) metodos.Add("Efectivo");
            if (tarjeta) metodos.Add("Tarjeta");
            if (transfer) metodos.Add("Transferencia");

            if (metodos.Any())
                query = query.Where(p => metodos.Contains(p.metodoDePago));

            var lista = await query
    .Select(p => new HistorialPagoDTO
    {
        Id = p.id,
        FechaPago = p.fecha,
        Monto = p.monto,
        MetodoDePago = p.metodoDePago,

        ClienteNombre = p.turno.cliente.nombre,
        ClienteApellido = p.turno.cliente.apellido,

        FechaTurno = p.turno.fecha,
        ServicioDetalle = p.turno.servicio.servicioBase.detalle,

        PromoDescripcion = p.turno.promo.id != 3 ? p.turno.promo.descripcion : null
    })
    .ToListAsync();

            return Ok(lista);
        }



    }
}