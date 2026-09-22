using NexCode.Model;

namespace NexCode.Controller
{
    public class ControladorReintento
    {
        private readonly Nivel nivel;

        public ControladorReintento(Nivel nivel)
        {
            this.nivel = nivel;
        }

        public void RegistrarFalloEnNivel()
        {
            nivel.RegistrarFallo();
        }

        public bool NecesitaApoyoExtra()
        {
            return nivel.NecesitaApoyo;
        }
    }
}