using NexCode.Model;

namespace NexCode.Controller
{
    public class ControladorProgreso
    {
        private readonly Progreso progreso;

        public ControladorProgreso (Progreso progreso)
        {
            this.progreso = progreso;
        }

        public void CompletarNivel(Nivel nivel)
        {
            progreso.ActualizarNivel(nivel);
        }

        public float ObtenerPorcentajeAvance(int totalNivelesRuta)
        {
            return progreso.CalcularPorcentaje(totalNivelesRuta);
        }
    }
}