using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Контроллер UI внутри игры
/// </summary>
public class UiController : MonoBehaviour
{
    #region Singleton
    public static UiController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Ui controller instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    private const string MENUSCENE = "Menu";

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverTitle;
    [SerializeField] private Text gameOverInfoText;

    [SerializeField] private string loseText;
    [SerializeField] private string winText;
    [SerializeField] private string winAdditionalText;
    [SerializeField] private string loseAdditionalText;

    public static bool IsUserInMenu;

    /// <summary>
    /// Изначально user всегда начинает не из меню
    /// </summary>
    private void OnEnable()
    {
        IsUserInMenu = false;
    }

    /// <summary>
    /// Ждет один фрейм, потом ставит IsUserInMenu в значение
    /// </summary>
    /// <param name="value">Значение для подстановки в IsUserInMenu</param>
    private IEnumerator UserInMenuCoroutine(bool value)
    {
        yield return null;
        IsUserInMenu = value;
    }

    /// <summary>
    /// Вызывает экран геймовера
    /// </summary>
    /// <param name="isPlayerWon">Выиграл ли игрок?</param>
    public void SummonGameOverScreen(bool isPlayerWon)
    {
        IsUserInMenu = true;
        if (isPlayerWon)
        {
            gameOverTitle.text = winText;
            gameOverInfoText.text = winAdditionalText;
        }
        else
        {
            gameOverTitle.text = loseText;
            gameOverInfoText.text = loseAdditionalText;
        }
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Вызывает/закрывает внутриигровое меню
    /// </summary>
    /// <param name="activate">Активировать меню?</param>
    public void SummonMenu(bool activate = true)
    {
        menuPanel.SetActive(activate);
        StartCoroutine(UserInMenuCoroutine(activate));
    }

    /// <summary>
    /// Загружает в главное меню
    /// </summary>
    public void GotoMainMenu()
    {
        SceneManager.LoadScene(MENUSCENE);
    }

    /// <summary>
    /// Рестарт уровня
    /// </summary>
    public void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Находится ли указатель мыши над UI элементом?
    /// </summary>
    /// <returns>true, если находится, false - иначе</returns>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
