using System;

namespace NexCode.Model
{
    public class Retroalimentacion
    {
        public string IdRetroalimentacion { get; set; }
        public string Mensaje { get; private set; }
        public string Explicacion { get; private set; }
        public string Pista { get; private set; }
        public string Sugerencia { get; private set; }

        public Retroalimentacion(string idRetroalimentacion)
        {
            IdRetroalimentacion = idRetroalimentacion;
            Mensaje = string.Empty;
            Explicacion = string.Empty;
            Pista = string.Empty;
            Sugerencia = string.Empty;
        }

        public void GenerarError(string error)
        {
            switch (error)
            {
                case "respuesta_incorrecta":
                    Mensaje = "Tu respuesta todavía no es correcta";
                    Explicacion = "Revisa la lógica antes de intentar de nuevo";
                    Pista = "¿Ya probaste seguir el proceso paso a paso?";
                    break;

                case "orden_invertido":
                    Mensaje = "El orden de los pasos no es el correcto";
                    Explicacion = "Algunos pasos van antes de lo que pusiste";
                    Pista = "Piensa cuál paso depende del resultado de otro";
                    break;

                default:
                    Mensaje = "Hubo un error, intenta de nuevo.";
                    Explicacion = string.Empty;
                    Pista = string.Empty;
                    break;
            }
            
            Sugerencia = "Puedes pedir una pista si sigues atascado";
        }

        public string ObtenerPista()
        {
            return Pista;
        }

    }
    
}