using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    [SerializeField]
    private string baseUrl = "http://localhost:3000";

    [Serializable]
    private class RegisterRequest
    {
        public string displayName;
        public string email;
        public string password;
    }

    [Serializable]
    private class LoginRequest
    {
        public string email;
        public string password;
    }

    [Serializable]
    public class UserData
    {
        public string id;
        public string displayName;
        public string email;
        public int points;
    }

    [Serializable]
    public class AuthResponse
    {
        public string message;
        public string token;
        public UserData user;
    }

    [Serializable]
    private class ErrorResponse
    {
        public string message;
    }

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

    public void Register(
        string username,
        string password,
        Action<bool, string> callback)
    {
        string normalizedUsername =
            username.Trim().ToLowerInvariant();

        RegisterRequest data = new RegisterRequest
        {
            displayName = username.Trim(),
            email = normalizedUsername + "@nexcode.local",
            password = password
        };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthenticationRequest(
                "/api/auth/register",
                json,
                callback
            )
        );
    }

    public void Login(
        string username,
        string password,
        Action<bool, string> callback)
    {
        string normalizedUsername =
            username.Trim().ToLowerInvariant();

        LoginRequest data = new LoginRequest
        {
            email = normalizedUsername + "@nexcode.local",
            password = password
        };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthenticationRequest(
                "/api/auth/login",
                json,
                callback
            )
        );
    }

    private IEnumerator SendAuthenticationRequest(
        string endpoint,
        string json,
        Action<bool, string> callback)
    {
        string url = baseUrl + endpoint;
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(url, "POST");

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        string responseText =
            request.downloadHandler.text;

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResponse response =
                JsonUtility.FromJson<AuthResponse>(
                    responseText
                );

            if (
                response == null ||
                string.IsNullOrEmpty(response.token)
            )
            {
                callback(
                    false,
                    "El servidor respondió sin una sesión válida."
                );

                yield break;
            }

            PlayerPrefs.SetString(
                "AuthToken",
                response.token
            );

            if (response.user != null)
            {
                PlayerPrefs.SetString(
                    "CurrentUserId",
                    response.user.id
                );

                PlayerPrefs.SetString(
                    "CurrentUsername",
                    response.user.displayName
                );
            }

            PlayerPrefs.Save();

            callback(
                true,
                response.message
            );
        }
        else
        {
            string message =
                "No se pudo conectar con el servidor.";

            if (!string.IsNullOrEmpty(responseText))
            {
                ErrorResponse error =
                    JsonUtility.FromJson<ErrorResponse>(
                        responseText
                    );

                if (
                    error != null &&
                    !string.IsNullOrEmpty(error.message)
                )
                {
                    message = error.message;
                }
            }
            else if (!string.IsNullOrEmpty(request.error))
            {
                message = request.error;
            }

            callback(false, message);
        }
    }

    public string GetToken()
    {
        return PlayerPrefs.GetString("AuthToken", "");
    }

    public bool HasSession()
    {
        return !string.IsNullOrEmpty(GetToken());
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("AuthToken");
        PlayerPrefs.DeleteKey("CurrentUserId");
        PlayerPrefs.DeleteKey("CurrentUsername");
        PlayerPrefs.Save();
    }
}