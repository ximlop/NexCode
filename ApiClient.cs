using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    [SerializeField]
    private string baseUrl = "https://localhost:5001/api";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Serializable]
    public class ProgresoUsuario
    {
        public int Puntos;
        public int Racha;
        public int NivelActual;
        public string[] NivelesCompletados;
    }

    [Serializable]
    private class CompletarNivelRequest
    {
        public string UsuarioId;
        public string NivelId;
        public int Puntos;
    }

    [Serializable]
    private class IntentoRequest
    {
        public string UsuarioId;
        public string RespuestaEnviada;
    }

    [Serializable]
    public class IntentoResponse
    {
        public bool Correcto;
        public int PuntosObtenidos;
        public string Retroalimentacion;
        public bool PuedeReintentar;
    }

    public void ObtenerProgreso(
        string usuarioId,
        Action<ProgresoUsuario> onExito,
        Action<string> onError = null)
    {
        StartCoroutine(ObtenerProgresoRutina(usuarioId, onExito, onError));
    }

    private IEnumerator ObtenerProgresoRutina(
        string usuarioId,
        Action<ProgresoUsuario> onExito,
        Action<string> onError)
    {
        string url = $"{baseUrl}/progreso?usuarioId={usuarioId}";

        using UnityWebRequest peticion = UnityWebRequest.Get(url);
        yield return peticion.SendWebRequest();

        if (peticion.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(peticion.error);
            yield break;
        }

        ProgresoUsuario progreso = JsonUtility.FromJson<ProgresoUsuario>(
            peticion.downloadHandler.text);
        onExito?.Invoke(progreso);
    }

    public void CompletarNivel(
        string usuarioId,
        string nivelId,
        int puntos,
        Action<ProgresoUsuario> onExito,
        Action<string> onError = null)
    {
        var body = new CompletarNivelRequest
        {
            UsuarioId = usuarioId,
            NivelId = nivelId,
            Puntos = puntos
        };

        StartCoroutine(PostRutina(
            $"{baseUrl}/progreso/completar-nivel",
            JsonUtility.ToJson(body),
            onExito,
            onError));
    }

    public void EnviarIntento(
        string desafioId,
        string usuarioId,
        string respuestaEnviada,
        Action<IntentoResponse> onExito,
        Action<string> onError = null)
    {
        var body = new IntentoRequest
        {
            UsuarioId = usuarioId,
            RespuestaEnviada = respuestaEnviada
        };

        StartCoroutine(PostRutina(
            $"{baseUrl}/desafios/{desafioId}/intentos",
            JsonUtility.ToJson(body),
            onExito,
            onError));
    }

    private IEnumerator PostRutina<T>(
        string url,
        string jsonBody,
        Action<T> onExito,
        Action<string> onError)
    {
        using var peticion = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        peticion.uploadHandler = new UploadHandlerRaw(bodyRaw);
        peticion.downloadHandler = new DownloadHandlerBuffer();
        peticion.SetRequestHeader("Content-Type", "application/json");

        yield return peticion.SendWebRequest();

        if (peticion.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(peticion.error);
            yield break;
        }

        T respuesta = JsonUtility.FromJson<T>(peticion.downloadHandler.text);
        onExito?.Invoke(respuesta);
    }
}
