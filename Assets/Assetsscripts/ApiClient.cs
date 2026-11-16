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
    private class ChangePasswordRequest
    {
        public string currentPassword;
        public string newPassword;
    }

    [Serializable]
    private class LearningPathRequest
    {
        public string learningPath;
    }

    [Serializable]
    private class ProgressRequest
    {
        public string learningPath;
        public int levelNumber;
    }

    [Serializable]
    public class UserSettingsData
    {
        public string learningPath;
        public bool tutorialCompleted;
    }

    [Serializable]
    public class PathProgressData
    {
        public int highestUnlockedLevel;
        public int[] completedLevels;
    }

    [Serializable]
    public class ProgressData
    {
        public PathProgressData Python;
        public PathProgressData Java;
    }

    [Serializable]
    public class UserData
    {
        public string id;
        public string displayName;
        public string email;
        public int points;
        public UserSettingsData settings;
        public ProgressData progress;
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

    [Serializable]
    private class ApiMessageResponse
    {
        public string message;
        public int pointsAwarded;
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
                CacheUserData(response.user);
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

    public void SaveCompletedLevel(
        string learningPath,
        int levelNumber,
        Action<bool, string> callback)
    {
        ProgressRequest data = new ProgressRequest
        {
            learningPath = learningPath,
            levelNumber = levelNumber
        };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthorizedPost(
                "/api/user/progress/complete",
                json,
                callback
            )
        );
    }

    public void SaveMistake(
        string learningPath,
        int levelNumber,
        Action<bool, string> callback)
    {
        ProgressRequest data = new ProgressRequest
        {
            learningPath = learningPath,
            levelNumber = levelNumber
        };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthorizedPost(
                "/api/user/mistakes",
                json,
                callback
            )
        );
    }

    private IEnumerator SendAuthorizedPost(
        string endpoint,
        string json,
        Action<bool, string> callback)
    {
        string token = GetToken();

        if (string.IsNullOrEmpty(token))
        {
            callback(
                false,
                "No existe una sesión activa."
            );

            yield break;
        }

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

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + token
        );

        yield return request.SendWebRequest();

        string responseText =
            request.downloadHandler.text;

        ApiMessageResponse response = null;

        if (!string.IsNullOrEmpty(responseText))
        {
            response =
                JsonUtility.FromJson<ApiMessageResponse>(
                    responseText
                );
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (
                response != null &&
                response.pointsAwarded > 0
            )
            {
                int currentPoints =
                    PlayerPrefs.GetInt(
                        "CurrentPoints",
                        0
                    );

                PlayerPrefs.SetInt(
                    "CurrentPoints",
                    currentPoints +
                    response.pointsAwarded
                );

                PlayerPrefs.Save();
            }

            string message =
                response != null &&
                !string.IsNullOrEmpty(response.message)
                    ? response.message
                    : "Datos guardados.";

            callback(true, message);
        }
        else
        {
            string message =
                "No se pudieron guardar los datos.";

            if (
                response != null &&
                !string.IsNullOrEmpty(response.message)
            )
            {
                message = response.message;
            }
            else if (!string.IsNullOrEmpty(request.error))
            {
                message = request.error;
            }

            callback(false, message);
        }
    }

    private void CacheUserData(UserData user)
    {
        ClearLocalLearningData();

        PlayerPrefs.SetString(
            "CurrentUserId",
            user.id ?? ""
        );

        PlayerPrefs.SetString(
            "CurrentUsername",
            user.displayName ?? ""
        );

        PlayerPrefs.SetInt(
            "CurrentPoints",
            user.points
        );

        if (user.settings != null)
        {
            PlayerPrefs.SetString(
                "LearningPath",
                user.settings.learningPath ?? ""
            );

            PlayerPrefs.SetInt(
                "TutorialCompleted",
                user.settings.tutorialCompleted ? 1 : 0
            );
        }

        if (user.progress != null)
        {
            CachePathProgress(
                "Python",
                user.progress.Python
            );

            CachePathProgress(
                "Java",
                user.progress.Java
            );
        }

        PlayerPrefs.Save();
    }

    private void CachePathProgress(
        string learningPath,
        PathProgressData progress)
    {
        int highestUnlockedLevel = 1;

        if (progress != null)
        {
            highestUnlockedLevel =
                Mathf.Max(
                    1,
                    progress.highestUnlockedLevel
                );
        }

        PlayerPrefs.SetInt(
            "HighestUnlockedLevel_" + learningPath,
            highestUnlockedLevel
        );

        if (
            progress == null ||
            progress.completedLevels == null
        )
        {
            return;
        }

        foreach (int completedLevel in progress.completedLevels)
        {
            PlayerPrefs.SetInt(
                "Completed_" +
                learningPath +
                "_" +
                completedLevel,
                1
            );
        }
    }

    private void ClearLocalLearningData()
    {
        string[] learningPaths =
        {
            "Python",
            "Java"
        };

        PlayerPrefs.DeleteKey("LearningPath");

        foreach (string learningPath in learningPaths)
        {
            PlayerPrefs.DeleteKey(
                "HighestUnlockedLevel_" + learningPath
            );

            PlayerPrefs.DeleteKey(
                "PathCompleted_" + learningPath
            );

            for (int level = 1; level <= 6; level++)
            {
                PlayerPrefs.DeleteKey(
                    "Completed_" +
                    learningPath +
                    "_" +
                    level
                );

                PlayerPrefs.DeleteKey(
                    "Mistakes_" +
                    learningPath +
                    "_" +
                    level
                );
            }
        }
    }

    public void SaveLearningPath(
        string learningPath,
        Action<bool, string> callback)
    {
        LearningPathRequest data =
            new LearningPathRequest
            {
                learningPath = learningPath
            };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthorizedPatch(
                "/api/user/settings",
                json,
                callback
            )
        );
    }

    private IEnumerator SendAuthorizedPatch(
        string endpoint,
        string json,
        Action<bool, string> callback)
    {
        string token = GetToken();

        if (string.IsNullOrEmpty(token))
        {
            callback(
                false,
                "No existe una sesión activa."
            );

            yield break;
        }

        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                baseUrl + endpoint,
                "PATCH"
            );

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + token
        );

        yield return request.SendWebRequest();

        string responseText =
            request.downloadHandler.text;

        ApiMessageResponse response = null;

        if (!string.IsNullOrEmpty(responseText))
        {
            response =
                JsonUtility.FromJson<ApiMessageResponse>(
                    responseText
                );
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string message =
                response != null &&
                !string.IsNullOrEmpty(response.message)
                    ? response.message
                    : "Configuración guardada.";

            callback(true, message);
        }
        else
        {
            string message =
                response != null &&
                !string.IsNullOrEmpty(response.message)
                    ? response.message
                    : "No se pudo guardar el camino.";

            callback(false, message);
        }
    }

    public void ResetAccountProgress(
        Action<bool, string> callback)
    {
        StartCoroutine(
            SendAuthorizedPost(
                "/api/user/reset-progress",
                "{}",
                callback
            )
        );
    }

    public void ChangePassword(
        string currentPassword,
        string newPassword,
        Action<bool, string> callback)
    {
        ChangePasswordRequest data =
            new ChangePasswordRequest
            {
                currentPassword = currentPassword,
                newPassword = newPassword
            };

        string json = JsonUtility.ToJson(data);

        StartCoroutine(
            SendAuthorizedRequest(
                "/api/user/password",
                "PATCH",
                json,
                callback
            )
        );
    }

    private IEnumerator SendAuthorizedRequest(
        string endpoint,
        string method,
        string json,
        Action<bool, string> callback)
    {
        string token = GetToken();

        if (string.IsNullOrEmpty(token))
        {
            callback(false, "Debes iniciar sesión.");
            yield break;
        }

        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(baseUrl + endpoint, method);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + token
        );

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        string message = request.error;

        if (!string.IsNullOrEmpty(responseText))
        {
            ErrorResponse response =
                JsonUtility.FromJson<ErrorResponse>(responseText);

            if (
                response != null &&
                !string.IsNullOrEmpty(response.message)
            )
            {
                message = response.message;
            }
        }

        bool successful =
            request.result == UnityWebRequest.Result.Success;

        callback(successful, message);
    }
    public string GetToken()
    {
        return PlayerPrefs.GetString(
            "AuthToken",
            ""
        );
    }

    public bool HasSession()
    {
        return !string.IsNullOrEmpty(
            GetToken()
        );
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("AuthToken");
        PlayerPrefs.DeleteKey("CurrentUserId");
        PlayerPrefs.DeleteKey("CurrentUsername");
        PlayerPrefs.DeleteKey("CurrentPoints");
        PlayerPrefs.DeleteKey("TutorialCompleted");

        ClearLocalLearningData();

        PlayerPrefs.Save();
    }
}