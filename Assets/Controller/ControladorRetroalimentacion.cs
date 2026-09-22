using NexCode.Model;

namespace NexCode.Controller
{
    public class ControladorRetroalimentacion
    {
        private readonly Retroalimentacion retroalimentacion;

        public ControladorRetroalimentacion (Retroalimentacion retroalimentacion)
        {
            this.retroalimentacion = retroalimentacion;
        }

        public void GenerarRetroalimentacionPorError(string tipoError)
        {
            retroalimentacion.GenerarParaError(tipoError);
        }

        public string ObtenerPista()
        {
            return retroalimentacion.ObtenerPista();
        }
    }
}