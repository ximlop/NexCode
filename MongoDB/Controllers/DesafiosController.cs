using Microsoft.AspNetCore.Mvc;
using NexCode.Api.Models;
using NexCode.Api.Services;

namespace NexCode.Api.Controllers
{
    [ApiController]
    [Route("api/desafios")]
    public class DesafiosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public DesafiosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public record IntentoRequest(string UsuarioId, string RespuestaEnviada);

        public record IntentoResponse(
            bool Correcto,
            int PuntosObtenidos,
            string Retroalimentacion,
            bool PuedeReintentar);

        
        [HttpPost("{desafioId}/intentos")]
        public async Task<ActionResult<IntentoResponse>> RegistrarIntento(
            string desafioId,
            [FromBody] IntentoRequest request)
        {
            
            bool esCorrecto = EvaluarRespuesta(desafioId, request.RespuestaEnviada);
            int puntos = esCorrecto ? 10 : 0;

            var resultado = new ResultadoDesafio
            {
                UsuarioId = request.UsuarioId,
                DesafioId = desafioId,
                Correcto = esCorrecto,
                NumeroIntento = 1, 
                PuntosObtenidos = puntos
            };

            await _usuarioService.RegistrarResultadoAsync(resultado);

            var respuesta = new IntentoResponse(
                Correcto: esCorrecto,
                PuntosObtenidos: puntos,
                Retroalimentacion: esCorrecto
                    ? "¡Correcto!"
                    : "Revisa la condición utilizada en el ciclo.",
                PuedeReintentar: true);

            return Ok(respuesta);
        }

       
        [HttpGet("fallidos")]
        public async Task<ActionResult<List<ResultadoDesafio>>> ObtenerFallidos(
            [FromQuery] string usuarioId)
        {
            var fallidos = await _usuarioService.ObtenerFallidosAsync(usuarioId);
            return Ok(fallidos);
        }

        private bool EvaluarRespuesta(string desafioId, string respuestaEnviada)
        {
            
            return false;
        }
    }
}
