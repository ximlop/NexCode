using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsUIController : MonoBehaviour
{
    [Header("Ventana de puntos")]
    [SerializeField] private GameObject popupLayer;
    [SerializeField] private TMP_Text pointsValueText;

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
}