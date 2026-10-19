using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsUIController : MonoBehaviour
{
    [Header("Ventana de puntos")]
    [SerializeField] private GameObject popupLayer;
    [SerializeField] private TMP_Text pointsValueText;

    [Header("Ventana de apariencia")]
    [SerializeField] private GameObject appearancePopupLayer;

    [Header("Elementos afectados por el tema")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image contentImage;
    [SerializeField] private Image accountCardImage;
    [SerializeField] private Image bottomNavigationImage;

    private void Start()
    {
        bool darkMode = PlayerPrefs.GetInt("DarkMode", 0) == 1;
        ApplyTheme(darkMode);
    }

    public void OpenPointsPopup()
    {
        int totalPoints = PlayerPrefs.GetInt("TotalPoints", 0);

        pointsValueText.text = totalPoints.ToString();
        popupLayer.SetActive(true);
    }

    public void ClosePointsPopup()
    {
        popupLayer.SetActive(false);
    }

    public void OpenAppearancePopup()
    {
        appearancePopupLayer.SetActive(true);
    }

    public void SelectLightTheme()
    {
        PlayerPrefs.SetInt("DarkMode", 0);
        PlayerPrefs.Save();

        ApplyTheme(false);
        appearancePopupLayer.SetActive(false);
    }

    public void SelectDarkTheme()
    {
        PlayerPrefs.SetInt("DarkMode", 1);
        PlayerPrefs.Save();

        ApplyTheme(true);
        appearancePopupLayer.SetActive(false);
    }

    public void CloseAppearancePopup()
    {
        appearancePopupLayer.SetActive(false);
    }

    private void ApplyTheme(bool darkMode)
    {
        if (darkMode)
        {
            backgroundImage.color = new Color32(24, 27, 33, 255);
            contentImage.color = new Color32(35, 39, 46, 255);
            accountCardImage.color = new Color32(48, 53, 61, 255);
            bottomNavigationImage.color = new Color32(20, 23, 28, 255);
        }
        else
        {
            backgroundImage.color = new Color32(247, 245, 235, 255);
            contentImage.color = new Color32(238, 238, 238, 255);
            accountCardImage.color = new Color32(255, 255, 255, 255);
            bottomNavigationImage.color = new Color32(245, 245, 245, 255);
        }
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

    public void GoBack()
    {
        SceneManager.LoadScene("Progreso");
    }
}