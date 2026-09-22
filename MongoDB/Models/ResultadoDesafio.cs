using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexCode.Api.Models
{
    public class ResultadoDesafio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioId { get; set; }

        public string DesafioId { get; set; }
        public bool Correcto { get; set; }
        public int NumeroIntento { get; set; }
        public int PuntosObtenidos { get; set; }
        public DateTime Fecha { get; set; }
    }
}
