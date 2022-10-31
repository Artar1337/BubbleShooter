using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер UI внутри главного меню
/// </summary>
public class MainMenuController : MonoBehaviour
{
    private const string GAMELEVEL = "Game";

    [SerializeField] private Image[] bubbles;
    [SerializeField] private Color[] bubbleColors;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelPanel;

    private void OnEnable()
    {
        RandomizeBubbles();
    }

    /// <summary>
    /// Задает случайные цвета для images
    /// </summary>
    private void RandomizeBubbles()
    {
        foreach (Image img in bubbles)
        {
            img.color = bubbleColors[Random.Range(0, bubbleColors.Length)];
        }
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Грузит в случайный уровень
    /// </summary>
    public void LoadRandomLevel()
    {
        LoadGame();
    }

    /// <summary>
    /// Активирует экран выбора уровня
    /// </summary>
    public void LoadLevelChooseScreen()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    /// <summary>
    /// Активирует экран выбора режима игры
    /// </summary>
    public void LoadMainScreen()
    {
        menuPanel.SetActive(true);
        levelPanel.SetActive(false);
    }

    /// <summary>
    /// Загружает уровень из файла по индексу
    /// </summary>
    /// <param name="index">Индекс уровня</param>
    public void LoadLevelByIndex(int index)
    {
        LoadGame();
    }

    /// <summary>
    /// Загружает игровой экран
    /// </summary>
    private void LoadGame()
    {
        SceneManager.LoadScene(GAMELEVEL);
    }
}
