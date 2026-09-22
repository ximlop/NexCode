using System;

namespace NexCode.Model
{
    public class DesafioDiario
    {
        public string IdDesafio { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; private set; }
        public int Meta { get; set; }
        public int Avance { get; private set; }
        public bool Completado { get; private set; }

        public DesafioDiario(string idDesafio, string descripcion, int meta)
        {
            IdDesafio = idDesafio;
            Descripcion = descripcion;
            Meta = meta;
            Fecha = DateTime.Now;
            Avance = 0;
            Completado = false;
        }

        public void ActualizarAvance(int incremento)
        {
            if (Completado)
            {
                return;
            }
            Avance += incremento;

            if (Avance >= Meta)
            {
                Avance = Meta;
                Completado = true;
            }

        }
        
        public bool VerificarCumplimiento()
        {
            return Completado;
        }
    }
}