
namespace NexCode.Model
{
    public class Usuario
    {
        public string IdUsuario { get; set; }
        public string Nombre { get; private set; }
        public string Correo { get; private set; }
        public string PasswordHash { get; private set; }
        public int Puntos { get; private set; }
        public int NivelGeneral { get; private set; }

        public Usuario(string idUsuario, string nombre, string correo, string passwordHash)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Correo = correo;
            PasswordHash = passwordHash;
            Puntos = 0;
            NivelGeneral = 1;
        }

        public void AgregarPuntos(int puntos)
        {
            if (puntos < 0)
            {
                throw new System.ArgumentException("Los puntos que se agregan no pueden ser negativos");
            }
            Puntos += puntos;
        }

        public void ActualizarPerfil(string nombre, string correo)
        {
            Nombre = nombre;
            Correo = correo;
        }
    }
    
}