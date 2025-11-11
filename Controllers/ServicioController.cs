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
    public class ServicioController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public ServicioController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.config = config;
            environment = env;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServicioPropioDTO>>> getServiciosPropios()
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            var serviciosPropios = await contexto.ServicioPropio
      .Include(sp => sp.servicioBase)
      .Where(sp => sp.usuarioId == usuarioId)
      .Select(sp => new ServicioPropioDTO
      {
          Id = sp.id,
          PrecioBase = sp.precioBase,
          DuracionMinutos = sp.duracionMinutos,
          Detalle = sp.servicioBase.detalle,
          Categoria = sp.servicioBase.categoria
      })
      .ToListAsync();

            if (serviciosPropios == null || serviciosPropios.Count == 0)
            {
                return Ok(new List<ServicioPropioDTO>());
            }
            return Ok(serviciosPropios);
        }


        [HttpGet("serviciosBase")]
        public async Task<ActionResult<List<ServicioBase>>> GetServiciosBase()
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            var servicios = await contexto.ServicioBase
                .Where(sb => sb.id != 1)
                .OrderBy(sb => sb.categoria)
                .ToListAsync();

            return Ok(servicios);
        }

        [HttpPost("crear")]
public async Task<IActionResult> crearServicioPropio([FromBody] ServicioPropioDTO dto)
{
    try
    {
        if (dto == null)
            return BadRequest("Datos inválidos.");

        var idClaim = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(idClaim))
            return Unauthorized("Token inválido.");
        int usuarioId = int.Parse(idClaim);

        var servicioBase = await contexto.ServicioBase
            .FirstOrDefaultAsync(s =>
                s.detalle == dto.Detalle &&
                s.categoria == dto.Categoria);

        if (servicioBase == null)
            return StatusCode(404, "Error al encontrar el servicio base");

        var existe = await contexto.ServicioPropio
            .AnyAsync(sp => sp.usuarioId == usuarioId && sp.servicioId == servicioBase.id);

        if (existe)
            return Conflict("Ya existe un servicio propio del mismo tipo.");

        var servicioPropio = new ServicioPropio
        {
            usuarioId = usuarioId,
            servicioId = servicioBase.id,
            precioBase = dto.PrecioBase,
            duracionMinutos = dto.DuracionMinutos
        };

        contexto.ServicioPropio.Add(servicioPropio);
        await contexto.SaveChangesAsync();

        // 🔹 Crear DTO de respuesta
        var dtoRespuesta = new ServicioPropioDTO
        {
            Id = servicioPropio.id,
            Categoria = dto.Categoria,
            Detalle = dto.Detalle,
            PrecioBase = servicioPropio.precioBase,
            DuracionMinutos = servicioPropio.duracionMinutos
        };

        return Ok(dtoRespuesta);
    }
    catch (Exception ex)
    {
        var inner = ex.InnerException?.Message;
        return StatusCode(500, "Error interno del servidor");
    }
}

[HttpPut("editar")]
public async Task<IActionResult> editarServicioPropio([FromBody] ServicioPropioDTO dto)
{
    try
    {
        if (dto == null)
            return BadRequest("Datos inválidos.");

        var idClaim = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(idClaim))
            return Unauthorized("Token inválido.");
        int usuarioId = int.Parse(idClaim);

        var servicioPropio = await contexto.ServicioPropio
            .FirstOrDefaultAsync(sp => sp.id == dto.Id && sp.usuarioId == usuarioId);

        if (servicioPropio == null)
            return StatusCode(404, "Error al encontrar el servicio propio");

        servicioPropio.precioBase = dto.PrecioBase;
        servicioPropio.duracionMinutos = dto.DuracionMinutos;

        await contexto.SaveChangesAsync();
        
        var dtoRespuesta = new ServicioPropioDTO
        {
            Id = servicioPropio.id,
            Categoria = dto.Categoria,
            Detalle = dto.Detalle,
            PrecioBase = servicioPropio.precioBase,
            DuracionMinutos = servicioPropio.duracionMinutos
        };

        return Ok(dtoRespuesta);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error interno del servidor");
    }
}



    }
}