using System;
using System.Security.Cryptography.X509Certificates;

namespace NexCode.Model
{
    
    public class Progreso
    {
        public string IdProgreso { get; set; }
        public string IdUsuario { get; set; }
        public int NivelesCompletados { get; private set; }
        public int PuntosAcumulados { get; private set; }
        public string RutaActual { get; set; }
        public DateTime FechaUltimaActividad { get; private set; }

        public Progreso(string idProgreso, string idUsuario, string rutaActual)
        {
            IdProgreso = idProgreso;
            IdUsuario = idUsuario;
            RutaActual = rutaActual;
            NivelesCompletados = 0;
            PuntosAcumulados = 0;
            FechaUltimaActividad = DateTime.Now;
        }

        public void ActualizarNivel(Nivel nivel)
        {
            if (nivel == null)
            {
                throw new ArgumentNullException(nameof(nivel), "El nivel no puede ser nuo");
            }

            NivelesCompletados++;
            PuntosAcumulados += nivel.PuntosRequeridos;
            FechaUltimaActividad = DateTime.Now;
        }

        public float CalcularPorcentaje(int totalNivelesRuta)
        {
            if(totalNivelesRuta <= 0)
            {
                return 0f;
            }

            return (float)NivelesCompletados / totalNivelesRuta * 100f;
        }

    }

}