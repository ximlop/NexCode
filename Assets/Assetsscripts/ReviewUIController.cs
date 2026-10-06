using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReviewUIController : MonoBehaviour
{
    [Header("Pantalla de repaso")]
    [SerializeField] private TMP_Text challengeStatusText;
    [SerializeField] private GameObject lessonPopupLayer;

    private void Start()
    {
        if (lessonPopupLayer != null)
        {
            lessonPopupLayer.SetActive(false);
        }

        challengeStatusText.text =
            "Aquí aparecerán tus niveles por reforzar.";
    }

    public void StartReview()
    {
        challengeStatusText.text =
            "El repaso estará disponible cuando conectemos "
            + "el registro de niveles con dificultad.";
    }

    public void OpenSettingsScene()
    {
        SceneManager.LoadScene("Ajustes");
    }

    public void OpenProgressScene()
    {
        SceneManager.LoadScene("Progreso");
    }

    public void OpenDailyScene()
    {
        SceneManager.LoadScene("Diario");
    }
}
