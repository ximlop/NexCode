using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexCode.Api.Models;

namespace NexCode.Api.Services
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly IMongoCollection<ResultadoDesafio> _resultados;

        public UsuarioService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _usuarios = database.GetCollection<Usuario>("Usuarios");
            _resultados = database.GetCollection<ResultadoDesafio>("Resultados");
        }

        public async Task<ProgresoUsuario?> ObtenerProgresoAsync(string usuarioId)
        {
            Usuario usuario = await _usuarios
                .Find(u => u.Id == usuarioId)
                .FirstOrDefaultAsync();

            return usuario?.Progreso;
        }

        public async Task CompletarNivelAsync(
            string usuarioId,
            string nivelId,
            int puntos)
        {
            var filtro = Builders<Usuario>.Filter.Eq(u => u.Id, usuarioId);

            var actualizacion = Builders<Usuario>.Update
                .AddToSet("Progreso.NivelesCompletados", nivelId)
                .Inc("Progreso.Puntos", puntos)
                .Set("Progreso.NivelActual", nivelId);

            await _usuarios.UpdateOneAsync(filtro, actualizacion);
        }

        public async Task RegistrarResultadoAsync(ResultadoDesafio resultado)
        {
            resultado.Fecha = DateTime.UtcNow;
            await _resultados.InsertOneAsync(resultado);
        }

        public async Task<List<ResultadoDesafio>> ObtenerFallidosAsync(string usuarioId)
        {
            return await _resultados
                .Find(r => r.UsuarioId == usuarioId && !r.Correcto)
                .SortByDescending(r => r.Fecha)
                .ToListAsync();
        }
    }
}
