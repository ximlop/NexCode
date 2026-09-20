using Microsoft.AspNetCore.Mvc;
using NexCode.Api.Models;
using NexCode.Api.Services;

namespace NexCode.Api.Controllers
{
    [ApiController]
    [Route("api/progreso")]
    public class ProgresoController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public ProgresoController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ProgresoUsuario>> ObtenerProgreso(
            [FromQuery] string usuarioId)
        {
            var progreso = await _usuarioService.ObtenerProgresoAsync(usuarioId);

            if (progreso == null)
            {
                return NotFound();
            }

            return Ok(progreso);
        }

        public record CompletarNivelRequest(
            string UsuarioId,
            string NivelId,
            int Puntos);
     
        [HttpPost("completar-nivel")]
        public async Task<IActionResult> CompletarNivel(
            [FromBody] CompletarNivelRequest request)
        {
            await _usuarioService.CompletarNivelAsync(
                request.UsuarioId,
                request.NivelId,
                request.Puntos);

            var progresoActualizado = await _usuarioService
                .ObtenerProgresoAsync(request.UsuarioId);

            return Ok(progresoActualizado);
        }
    }
}
