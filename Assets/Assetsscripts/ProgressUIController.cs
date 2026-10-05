using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressUIController : MonoBehaviour
{
    private const string LearningPathKey = "LearningPath";

    [Header("Selector de camino")]
    [SerializeField] private GameObject pathPopupLayer;
    [SerializeField] private TMP_Text currentPathText;

    [Header("Mapa de niveles")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Image[] pathLines;

    [Header("Colores")]
    [SerializeField] private Color unlockedColor =
        new Color32(35, 200, 90, 255);

    [SerializeField] private Color lockedColor =
        new Color32(145, 145, 145, 255);

    [Header("Ventana de ejercicio")]
    [SerializeField] private GameObject lessonPopupLayer;
    [SerializeField] private TMP_Text lessonTitleText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;

    private string activeLessonPath;
    private int activeLessonLevel;
    private int correctAnswerIndex;
    private bool lessonAnswered;

    private void Start()
    {
        pathPopupLayer.SetActive(false);
        lessonPopupLayer.SetActive(false);

        UpdatePathText();
        RefreshProgressMap();
    }

    public void OpenPathPopup()
    {
        pathPopupLayer.SetActive(true);
    }

    public void SelectPython()
    {
        SavePath("Python");
    }

    public void SelectJava()
    {
        SavePath("Java");
    }

    public void ClosePathPopup()
    {
        pathPopupLayer.SetActive(false);
    }

    private void SavePath(string selectedPath)
    {
        PlayerPrefs.SetString(LearningPathKey, selectedPath);

        string progressKey = GetProgressKey(selectedPath);

        if (!PlayerPrefs.HasKey(progressKey))
        {
            PlayerPrefs.SetInt(progressKey, 1);
        }

        PlayerPrefs.Save();

        UpdatePathText();
        RefreshProgressMap();
        pathPopupLayer.SetActive(false);
    }

    private void UpdatePathText()
    {
        string selectedPath =
            PlayerPrefs.GetString(LearningPathKey, "");

        currentPathText.text = string.IsNullOrEmpty(selectedPath)
            ? "Sin elegir"
            : selectedPath;
    }

    private void RefreshProgressMap()
    {
        string selectedPath =
            PlayerPrefs.GetString(LearningPathKey, "");

        if (string.IsNullOrEmpty(selectedPath))
        {
            SetAllLevelsLocked();
            return;
        }

        int highestUnlockedLevel =
            PlayerPrefs.GetInt(GetProgressKey(selectedPath), 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = i < highestUnlockedLevel;
            Button button = levelButtons[i];

            ColorBlock colors = button.colors;
            colors.normalColor = unlockedColor;
            colors.highlightedColor = unlockedColor;
            colors.selectedColor = unlockedColor;
            colors.pressedColor = unlockedColor * 0.8f;
            colors.disabledColor = lockedColor;
            button.colors = colors;

            button.interactable = isUnlocked;
        }

        for (int i = 0; i < pathLines.Length; i++)
        {
            bool lineUnlocked = i < highestUnlockedLevel - 1;

            pathLines[i].color = lineUnlocked
                ? unlockedColor
                : lockedColor;
        }
    }

    private void SetAllLevelsLocked()
    {
        foreach (Button button in levelButtons)
        {
            ColorBlock colors = button.colors;
            colors.disabledColor = lockedColor;
            button.colors = colors;
            button.interactable = false;
        }

        foreach (Image line in pathLines)
        {
            line.color = lockedColor;
        }
    }

    private string GetProgressKey(string selectedPath)
    {
        return "HighestUnlockedLevel_" + selectedPath;
    }

    public void OpenLevel(int levelNumber)
    {
        string selectedPath =
            PlayerPrefs.GetString(LearningPathKey, "");

        if (selectedPath != "Python" && selectedPath != "Java")
        {
            OpenPathPopup();
            return;
        }

        int highestUnlockedLevel =
            PlayerPrefs.GetInt(GetProgressKey(selectedPath), 1);

        if (levelNumber < 1 ||
            levelNumber > levelButtons.Length ||
            levelNumber > highestUnlockedLevel)
        {
            return;
        }

        activeLessonPath = selectedPath;
        activeLessonLevel = levelNumber;
        lessonAnswered = false;

        lessonTitleText.text =
            selectedPath + " · Nivel " + levelNumber;

        lessonPopupLayer.SetActive(true);

        if (levelNumber != 1)
        {
            lessonAnswered = true;
            questionText.text = "Este ejercicio todavía está en preparación.";
            feedbackText.text = "Puedes cerrar la ventana y volver al mapa.";

            foreach (Button button in answerButtons)
            {
                button.gameObject.SetActive(false);
            }

            return;
        }

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(true);
            button.interactable = true;
        }

        feedbackText.text = "Selecciona una respuesta.";
        feedbackText.color = Color.gray;

        if (selectedPath == "Python")
        {
            questionText.text =
                "¿Qué instrucción muestra Hola en la consola de Python?";

            answerTexts[0].text = "print(\"Hola\")";
            answerTexts[1].text = "input(\"Hola\")";
            answerTexts[2].text = "len(\"Hola\")";

            correctAnswerIndex = 0;
        }
        else
        {
            questionText.text =
                "¿Qué instrucción muestra Hola en la consola de Java?";

            answerTexts[0].text = "print(\"Hola\");";
            answerTexts[1].text = "System.out.println(\"Hola\");";
            answerTexts[2].text = "Console.WriteLine(\"Hola\");";

            correctAnswerIndex = 1;
        }
    }

    private void CompleteLevel(int completedLevel)
    {
    
        if (!lessonAnswered ||
            completedLevel != activeLessonLevel ||
            string.IsNullOrEmpty(activeLessonPath))
        {
            return;
        }

        string progressKey = GetProgressKey(activeLessonPath);
        int currentHighest = PlayerPrefs.GetInt(progressKey, 1);

        if (completedLevel < 1 ||
            completedLevel > levelButtons.Length ||
            completedLevel > currentHighest)
        {
            return;
        }

        int nextLevel = Mathf.Min(
            completedLevel + 1,
            levelButtons.Length
        );

        if (nextLevel > currentHighest)
        {
            PlayerPrefs.SetInt(progressKey, nextLevel);
            PlayerPrefs.Save();
        }

        RefreshProgressMap();
    }

    public void OpenSettingsScene()
    {
        SceneManager.LoadScene("Ajustes");
    }

    public void OpenDailyScene()
    {
        SceneManager.LoadScene("Diario");
    }

    public void OpenReviewScene()
    {
        SceneManager.LoadScene("Desafio");
    }

    public void SubmitAnswer(int answerIndex)
    {
        if (!lessonPopupLayer.activeSelf ||
            lessonAnswered ||
            activeLessonLevel != 1 ||
            answerIndex < 0 ||
            answerIndex >= answerButtons.Length)
        {
            return;
        }

        if (answerIndex != correctAnswerIndex)
        {
            feedbackText.color = new Color32(180, 45, 45, 255);
            feedbackText.text =
                "Respuesta incorrecta. Intenta de nuevo.";
            return;
        }

        lessonAnswered = true;

        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }

        string progressKey = GetProgressKey(activeLessonPath);
        bool unlocksNextLevel =
            activeLessonLevel == PlayerPrefs.GetInt(progressKey, 1) &&
            activeLessonLevel < levelButtons.Length;

        CompleteLevel(activeLessonLevel);

        feedbackText.color = new Color32(25, 130, 65, 255);
        feedbackText.text = unlocksNextLevel
            ? "¡Correcto! Has desbloqueado el nivel 2."
            : "¡Correcto! Completaste el repaso.";
    }

    public void CloseLessonPopup()
    {
        lessonPopupLayer.SetActive(false);
        activeLessonLevel = 0;
        activeLessonPath = "";
        lessonAnswered = false;
    }

}