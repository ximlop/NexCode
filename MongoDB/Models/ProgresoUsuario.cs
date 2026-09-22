namespace NexCode.Api.Models
{
    public class ProgresoUsuario
    {
        public int Puntos { get; set; }
        public int Racha { get; set; }
        public int NivelActual { get; set; }
        public List<string> NivelesCompletados { get; set; } = new();
    }
}
