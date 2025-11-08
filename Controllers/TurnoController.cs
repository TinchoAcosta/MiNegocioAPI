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
    public class TurnoController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public TurnoController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.config = config;
            environment = env;
        }

        [HttpGet("turnosDelDia")]
        public async Task<ActionResult<List<Turno>>> getTurnos([FromQuery] DateTime fecha)
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1).AddTicks(-1);

            var turnos = await contexto.Turno
            .Include(t => t.cliente)
            .Include(t => t.servicio)
                .ThenInclude(sp => sp.servicioBase)
            .Include(t => t.promo)
            .Include(t => t.pagos)
            .Where(t => t.servicio.usuarioId == usuarioId &&
                t.fecha >= fechaInicio &&
                t.fecha <= fechaFin)
            .Select(t => new TurnoDTO
            {
                Id = t.id,
                Fecha = t.fecha,
                Descripcion = t.descripcion,
                Estado = t.estado.ToString(),

                ClienteNombre = t.cliente.nombre + " " + t.cliente.apellido,
                ServicioNombre = t.servicio.servicioBase.detalle,
                Categoria = t.servicio.servicioBase.categoria,
                PrecioBase = t.servicio.precioBase,
                clienteId = t.clienteId,
                PromoDescripcion = t.promo.descripcion,
                PrecioPromo = t.promo != null ? t.promo.precioNuevo : null,

                Pagos = t.pagos.Select(p => new PagoDTO
                {
                    Fecha = p.fecha,
                    Monto = p.monto,
                    MetodoDePago = p.metodoDePago
                }).ToList()
            })
    .ToListAsync();

            if (!turnos.Any())
                return Ok(new List<TurnoDTO>());

            return Ok(turnos);
        }
    }
}