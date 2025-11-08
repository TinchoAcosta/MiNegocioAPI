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
    public class ClienteController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public ClienteController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.config = config;
            environment = env;
        }

        [HttpGet]
        public async Task<ActionResult<Cliente>> getCliente([FromQuery] int id)
        {
            var idClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized("Token inválido.");

            int usuarioId = int.Parse(idClaim);

            var cliente = await contexto.Cliente.FirstOrDefaultAsync(c => c.id == id);
            if (cliente == null)
                return NotFound("El cliente no existe.");

            bool tieneTurnoConUsuario = await contexto.Turno
                .Include(t => t.servicio)
                .AnyAsync(t => t.clienteId == id && t.servicio.usuarioId == usuarioId);

            if (!tieneTurnoConUsuario)
                return StatusCode(403, "No tenés permisos para ver este cliente.");

            return Ok(cliente);
        }
    }
}