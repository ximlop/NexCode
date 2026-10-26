using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUIController : MonoBehaviour
{
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

    public void OpenReviewScene()
    {
        SceneManager.LoadScene("Desafio");
    }

    public void OpenLogrosScene()
    {
        SceneManager.LoadScene("Logros");
    }

    public void FinishTutorial()
    {

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Progreso");
    }

}
