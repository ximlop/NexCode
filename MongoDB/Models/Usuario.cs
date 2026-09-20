using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexCode.Api.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Nombre { get; set; }
        public string CorreoNormalizado { get; set; }

        // Nunca se guarda la contraseña original.
        public string PasswordHash { get; set; }

        public int IntentosInicioFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }

        public ProgresoUsuario Progreso { get; set; } = new();
    }
}
