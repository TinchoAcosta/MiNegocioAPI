using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MiNegocioAPI.model;
using System.Text.Json;

namespace MiNegocioAPI.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class PromoController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public PromoController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.config = config;
            environment = env;
        }

        [HttpGet]
        public async Task<ActionResult<List<PromoDTO>>> GetPromos()
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            var promos = await contexto.Promo
                .Include(p => p.servicioPropio)
                    .ThenInclude(sp => sp.servicioBase)
                .Where(p => p.servicioPropio.usuarioId == usuarioId &&
                            p.estado == true &&
                            p.fechaFin >= DateTime.Now)
                .Select(p => new PromoDTO
                {
                    Id = p.id,
                    Descripcion = p.descripcion,
                    Imagen = p.imagen,
                    PrecioNuevo = p.precioNuevo,
                    Condicion = p.condicion,
                    FechaFin = p.fechaFin,
                    ServicioPropio = new ServicioPropioDTO(
                        p.servicioPropio.id,
                        p.servicioPropio.precioBase,
                        p.servicioPropio.duracionMinutos,
                        p.servicioPropio.servicioBase.detalle,
                        p.servicioPropio.servicioBase.categoria
                    )
                })
                .ToListAsync();

            return Ok(promos);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> cargarPromo([FromForm] IFormFile? imagen, [FromForm] string promo)
        {

            if (string.IsNullOrWhiteSpace(promo))
                return BadRequest("El JSON de la promo es requerido.");

            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            PromoDTO nuevaPromo;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                nuevaPromo = JsonSerializer.Deserialize<PromoDTO>(promo, options);
            }
            catch (JsonException ex)
            {
                return BadRequest($"Error al deserializar el JSON de la promo: {ex.Message}");
            }

            if (nuevaPromo == null)
                return BadRequest("No se pudo interpretar la promo.");

            if (nuevaPromo.ServicioPropio == null)
                return BadRequest("Debe seleccionar un servicio propio válido.");
            var servicio = await contexto.ServicioPropio
        .FirstOrDefaultAsync(sp => sp.id == nuevaPromo.ServicioPropio.Id && sp.usuarioId == usuarioId);

            if (servicio == null)
            {
                return Unauthorized("El servicio propio seleccionado no pertenece al usuario o no existe.");
            }

            var promoEntidad = new Promo
            {
                servicioPropioId = nuevaPromo.ServicioPropio.Id,
                descripcion = nuevaPromo.Descripcion,
                precioNuevo = nuevaPromo.PrecioNuevo,
                condicion = nuevaPromo.Condicion,
                fechaFin = nuevaPromo.FechaFin,
                estado = true
            };
            contexto.Promo.Add(promoEntidad);
            await contexto.SaveChangesAsync();

            if (imagen != null)
            {
                var uploadsFolder = Path.Combine(environment.WebRootPath, "promos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fileName = $"promo_foto{promoEntidad.id}.jpg";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(fileStream);
                }

                promoEntidad.imagen = $"/promos/{fileName}";
                contexto.Promo.Update(promoEntidad);
                await contexto.SaveChangesAsync();

            }

            return Ok(new
            {
                mensaje = "Promo creada correctamente",
                id = promoEntidad.id,
                imagen = promoEntidad.imagen
            });


        }

        [HttpPut("eliminar")]
        public async Task<IActionResult> EliminarPromo([FromBody]int id)
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            var promo = await contexto.Promo
                .Include(p => p.servicioPropio)
                .FirstOrDefaultAsync(p => p.id == id);
            if (promo == null)
                return NotFound();

            if (promo.servicioPropio.usuarioId != usuarioId)
                return Unauthorized();

            promo.estado = false;    

            contexto.Promo.Update(promo);
            await contexto.SaveChangesAsync();
            return Ok();
        }

    }
}