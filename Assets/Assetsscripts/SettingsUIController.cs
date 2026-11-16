using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsUIController : MonoBehaviour
{
    [Header("Ventana de puntos")]
    [SerializeField] private GameObject popupLayer;
    [SerializeField] private TMP_Text pointsValueText;

    [Header("Restablecer progreso")]
    [SerializeField]
    private GameObject resetAccountPopupLayer;

    [SerializeField]
    private Button confirmResetButton;

    [SerializeField]
    private TMP_Text resetStatusText;

    [Header("Cambio de contraseña")]
    [SerializeField] private GameObject changePasswordPopupLayer;
    [SerializeField] private TMP_InputField currentPasswordInput;
    [SerializeField] private TMP_InputField newPasswordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    [SerializeField] private TMP_Text passwordStatusText;
    [SerializeField] private Button savePasswordButton;

    private void Start()
    {
        resetAccountPopupLayer.SetActive(false);
    }

    public void OpenPointsPopup()
    {
        int totalPoints = PlayerPrefs.GetInt("CurrentPoints", 0);

        pointsValueText.text = totalPoints.ToString();
        popupLayer.SetActive(true);
    }

    public void ClosePointsPopup()
    {
        popupLayer.SetActive(false);
    }

    public void OpenResetAccountPopup()
    {
        resetStatusText.text = "";
        confirmResetButton.interactable = true;
        resetAccountPopupLayer.SetActive(true);
    }

    public void CloseResetAccountPopup()
    {
        resetAccountPopupLayer.SetActive(false);
    }

    public void ConfirmResetAccount()
    {
        if (
            ApiClient.Instance == null ||
            !ApiClient.Instance.HasSession()
        )
        {
            resetStatusText.text =
                "No existe una sesión activa.";

            return;
        }

        confirmResetButton.interactable = false;
        resetStatusText.text =
            "Restableciendo progreso...";

        ApiClient.Instance.ResetAccountProgress(
            OnResetAccountCompleted
        );
    }

    private void OnResetAccountCompleted(
        bool success,
        string message)
    {
        if (!success)
        {
            resetStatusText.text = message;
            confirmResetButton.interactable = true;
            return;
        }

        ClearLocalAccountProgress();

        if (pointsValueText != null)
        {
            pointsValueText.text = "0";
        }

        resetStatusText.text = "";


        resetAccountPopupLayer.SetActive(false);

        Debug.Log(
            "El progreso de la cuenta fue restablecido."
        );
    }

    private void ClearLocalAccountProgress()
    {
        PlayerPrefs.SetInt(
            "CurrentPoints",
            0
        );

        PlayerPrefs.SetString(
            "LearningPath",
            ""
        );

        string[] learningPaths =
        {
            "Python",
            "Java"
        };

        foreach (string learningPath in learningPaths)
        {
            PlayerPrefs.SetInt(
                "HighestUnlockedLevel_" + learningPath,
                1
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

        PlayerPrefs.Save();
    }

    public void OpenProgressScene()
    {
        SceneManager.LoadScene("Progreso");
    }

    public void OpenDailyScene()
    {
        SceneManager.LoadScene("Diario");
    }

    public void OpenTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenReviewScene()
    {
        SceneManager.LoadScene("Desafio");
    }

    public void OpenLogrosScene()
    {
        SceneManager.LoadScene("Logros");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Progreso");
    }

    public void OpenChangePasswordPopup()
    {
        currentPasswordInput.text = "";
        newPasswordInput.text = "";
        confirmPasswordInput.text = "";

        passwordStatusText.text = "";
        savePasswordButton.interactable = true;

        changePasswordPopupLayer.SetActive(true);
    }

    public void CloseChangePasswordPopup()
    {
        changePasswordPopupLayer.SetActive(false);
    }

    public void SubmitPasswordChange()
    {
        string currentPassword =
            currentPasswordInput.text;

        string newPassword =
            newPasswordInput.text;

        string confirmation =
            confirmPasswordInput.text;

        if (
            string.IsNullOrEmpty(currentPassword) ||
            string.IsNullOrEmpty(newPassword) ||
            string.IsNullOrEmpty(confirmation)
        )
        {
            ShowPasswordMessage(
                "Completa todos los campos.",
                false
            );

            return;
        }

        if (newPassword.Length < 8)
        {
            ShowPasswordMessage(
                "La contraseña debe tener al menos 8 caracteres.",
                false
            );

            return;
        }

        if (newPassword != confirmation)
        {
            ShowPasswordMessage(
                "Las contraseñas nuevas no coinciden.",
                false
            );

            return;
        }

        if (ApiClient.Instance == null)
        {
            ShowPasswordMessage(
                "No se encontró la conexión con el servidor.",
                false
            );

            return;
        }

        savePasswordButton.interactable = false;

        ShowPasswordMessage(
            "Actualizando contraseña...",
            true
        );

        ApiClient.Instance.ChangePassword(
            currentPassword,
            newPassword,
            OnPasswordChanged
        );
    }

    private void OnPasswordChanged(
        bool successful,
        string message)
    {
        savePasswordButton.interactable = true;

        ShowPasswordMessage(message, successful);

        if (!successful)
        {
            return;
        }

        currentPasswordInput.text = "";
        newPasswordInput.text = "";
        confirmPasswordInput.text = "";
    }

    private void ShowPasswordMessage(
        string message,
        bool successful)
    {
        passwordStatusText.text = message;

        passwordStatusText.color = successful
            ? new Color32(80, 210, 125, 255)
            : new Color32(235, 90, 90, 255);
    }
    public void Logout()
    {
        if (ApiClient.Instance != null)
        {
            ApiClient.Instance.Logout();
        }

        SceneManager.LoadScene("Autenticacion");
    }
}