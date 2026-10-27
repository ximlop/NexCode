using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DailyUIController : MonoBehaviour
{
    [Header("Misión: reto diario")]
    [SerializeField] private TMP_Text dailyProgressText;
    [SerializeField] private Slider dailyProgressBar;
    [SerializeField] private Button dailyClaimButton;

    [Header("Misión: aprobar lecciones")]
    [SerializeField] private TMP_Text lessonsProgressText;
    [SerializeField] private Slider lessonsProgressBar;
    [SerializeField] private Button lessonsClaimButton;

    [Header("Misión: hablar con tutores")]
    [SerializeField] private TMP_Text tutorsProgressText;
    [SerializeField] private Slider tutorsProgressBar;
    [SerializeField] private Button tutorsClaimButton;

    private void Start()
    {
        ShowMission(
            dailyProgressText, 
            dailyProgressBar, 
            dailyClaimButton, 
            0, 2
            );

        ShowMission(
            lessonsProgressText,
            lessonsProgressBar,
            lessonsClaimButton,
            0, 3
        );

        ShowMission(
            tutorsProgressText,
            tutorsProgressBar,
            tutorsClaimButton,
            0, 2
        );
    }

    private void ShowMission(
        TMP_Text progressText,
        Slider progressBar,
        Button claimButton,
        int current,
        int goal)
    {
        int displayedProgress = Mathf.Clamp(current, 0, goal);

        progressText.text = displayedProgress + " / " + goal;

        progressBar.minValue = 0;
        progressBar.maxValue = 1;
        progressBar.wholeNumbers = false;
        progressBar.value = (float)displayedProgress / goal;
        progressBar.interactable = false;

        claimButton.interactable = false;
    }

    public void OpenSettingsScene()
    {
        SceneManager.LoadScene("Ajustes");
    }

    public void OpenProgressScene()
    {
        SceneManager.LoadScene("Progreso");
    }

    public void OpenLogrosScene()
    {
        SceneManager.LoadScene("Logros");
    }

    public void OpenReviewScene()
    {
        SceneManager.LoadScene("Desafio");
    }
}
