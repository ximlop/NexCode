using System;

namespace NexCode.Model
{
    public class Nivel
    {
        private const int MaxIntentosFallidos = 3;

        public string IdNivel { get; set; }
        public string Titulo { get; set; }
        public int Numero { get; set; }
        public int Dificultad { get; set; }
        public int PuntosRequeridos { get; set; }
        public bool Bloqueado { get; private set; }
        public int IntentosFallidos { get; private set; }
        public bool NecesitaApoyo { get; private set; }

        public Nivel(string idNivel, string titulo, int numero, int dificultad, int puntosRequeridos)
        {
            IdNivel = idNivel;
            Titulo = titulo;
            Numero = numero;
            Dificultad = dificultad;
            PuntosRequeridos = puntosRequeridos;
            Bloqueado = true;
            IntentosFallidos = 0;
            NecesitaApoyo = false;
        }

        public void Iniciar()
        {
            if (Bloqueado)
            {
                throw new InvalidOperationException("No puedes iniciar un nivel bloqueado.");
            }
        }

        public void Completar()
        {
            IntentosFallidos = 0;
            NecesitaApoyo = false;
        }

        public void Desbloquear()
        {
            Bloqueado = false;
        }

        public void RegistrarFallo()
        {
            IntentosFallidos++;

            if (IntentosFallidos >= MaxIntentosFallidos)
            {
                NecesitaApoyo = true;
            }
        }
    }
}