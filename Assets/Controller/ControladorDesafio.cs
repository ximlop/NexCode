using NexCode.Model;

namespace NexCode.Controller
{
    public class ControladorDesafio{
        private readonly DesafioDiario desafio;

        public ControladorDesafio(DesafioDiario desafio)
        {
            this.desafio = desafio;
        }

        public void RegistrarResolucion(int incremento)
        {
            desafio.ActualizarAvance(incremento);
        }

        public bool EstaCompletado()
        {
            return desafio.VerificarCumplimiento();
        }
    }
    

}
