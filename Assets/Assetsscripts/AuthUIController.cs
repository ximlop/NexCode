using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthUIController : MonoBehaviour
{
    [Header("Ventanas")]
    [SerializeField] private GameObject loginPopupLayer;
    [SerializeField] private GameObject registerPopupLayer;

    [Header("Inicio de sesión")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_Text loginStatusText;
    [SerializeField] private Button loginButton;

    [Header("Registro")]
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    [SerializeField] private TMP_Text registerStatusText;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        loginPopupLayer.SetActive(false);
        registerPopupLayer.SetActive(false);

        loginStatusText.text = "";
        registerStatusText.text = "";
    }

    public void OpenLogin()
    {
        registerPopupLayer.SetActive(false);
        loginPopupLayer.SetActive(true);
        loginStatusText.text = "";
    }

    public void CloseLogin()
    {
        loginPopupLayer.SetActive(false);
        loginStatusText.text = "";
    }

    public void OpenRegister()
    {
        loginPopupLayer.SetActive(false);
        registerPopupLayer.SetActive(true);
        registerStatusText.text = "";
    }

    public void CloseRegister()
    {
        registerPopupLayer.SetActive(false);
        registerStatusText.text = "";
    }

    public void SubmitLogin()
    {
        string username =
            loginUsernameInput.text.Trim();

        string password =
            loginPasswordInput.text;

        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password))
        {
            loginStatusText.text =
                "Completa todos los campos.";

            return;
        }

        loginButton.interactable = false;
        loginStatusText.text = "Conectando...";

        ApiClient.Instance.Login(
            username,
            password,
            OnLoginCompleted
        );
    }

    private void OnLoginCompleted(
        bool success,
        string message)
    {
        loginButton.interactable = true;
        loginStatusText.text = message;

        if (success)
        {
            SceneManager.LoadScene("Progreso");
        }
    }

    public void SubmitRegister()
    {
        string username =
            registerUsernameInput.text.Trim();

        string password =
            registerPasswordInput.text;

        string confirmation =
            confirmPasswordInput.text;

        if (username.Length < 3)
        {
            registerStatusText.text =
                "El nombre debe tener al menos 3 caracteres.";

            return;
        }

        if (password.Length < 8)
        {
            registerStatusText.text =
                "La contraseña debe tener al menos 8 caracteres.";

            return;
        }

        if (password != confirmation)
        {
            registerStatusText.text =
                "Las contraseñas no coinciden.";

            return;
        }

        registerButton.interactable = false;
        registerStatusText.text =
            "Creando cuenta...";

        ApiClient.Instance.Register(
            username,
            password,
            OnRegisterCompleted
        );
    }

    private void OnRegisterCompleted(
        bool success,
        string message)
    {
        registerButton.interactable = true;
        registerStatusText.text = message;

        if (success)
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}