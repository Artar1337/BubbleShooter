using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер для пушки, отвечает за стрельбу, поворот, отрисовку линии
/// </summary>
public class CannonController : MonoBehaviour
{
    private const string MOUSEAXIS = "Fire1";
    private const float DEG90 = 90f;

    [SerializeField] private Camera mainCam;
    [SerializeField] private float degreeBorder = 60f;
    [SerializeField] private Transform bubbleSpawnRoot;
    [SerializeField] private Transform bubbleSpawnPoint;
    [SerializeField] private Image currentBall;
    [SerializeField] private Image nextBall;
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private float cannonCooldown = 1f;

    private bool cannonIsMoving = false;
    private LineRenderer lineRenderer;
    private BubbleColor currentColor;
    private BubbleColor nextColor;
    private int shotCount = 0;
    private bool canShootNow = true;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentColor = GenerateColor();
        nextColor = GenerateColor();
        UpdateUI();
    }

    /// <summary>
    /// Используется для временной невозможности управления пушкой
    /// </summary>
    private IEnumerator CannonCooldown()
    {
        canShootNow = false;
        yield return new WaitForSeconds(cannonCooldown);
        canShootNow = true;
    }

    /// <summary>
    /// Отвечает за INPUT, поворот пушки, выстрел и т.п.
    /// </summary>
    private void Update()
    {
        if (UiController.IsUserInMenu || !canShootNow)
        {
            return;
        }
        // Для Android
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 diff = mainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y)) - transform.position;
            diff.Normalize();
            transform.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Clamp(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - DEG90, -degreeBorder, degreeBorder));
            cannonIsMoving = true;
            LineRendererUpdate(false);
            return;
        }
        // Для ПК
        if (Input.GetAxis(MOUSEAXIS) > 0)
        {
            Vector3 diff = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z)) - 
                transform.position;
            diff.Normalize();
            transform.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Clamp(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - DEG90, -degreeBorder, degreeBorder));
            cannonIsMoving = true;
            LineRendererUpdate(false);
            return;
        }
        // Отпущена кнопка/палец
        if (cannonIsMoving)
        {
            cannonIsMoving = false;
            if (!UiController.IsPointerOverUIObject())
            {
                Shoot();
                currentColor = nextColor;
                nextColor = GenerateColor();
                UpdateUI();
            }
            transform.rotation = Quaternion.identity;
            LineRendererUpdate();
        }
    }

    /// <summary>
    /// Генерирует случайный цвет из BubbleColor
    /// </summary>
    /// <returns>Случайный цвет из BubbleColor</returns>
    private BubbleColor GenerateColor()
    {
        return (BubbleColor)Random.Range(1, System.Enum.GetNames(typeof(BubbleColor)).Length);
    }

    /// <summary>
    /// Стреляет пузырьком - пушечным шаром в текущем направлении поворота пушки
    /// </summary>
    private void Shoot()
    {
        Bubble bubble = Instantiate(BubbleGraph.instance.BubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, bubbleSpawnRoot).
            GetComponent<Bubble>();
        bubble.Initialize(true, transform.rotation * Vector2.up, currentColor);
        shotCount++;
        if((shotCount & 0x1) == 0x0)
        {
            BubbleGraph.instance.StartCoroutine(BubbleGraph.instance.AddOneLine());
        }
        StartCoroutine(CannonCooldown());
    }

    /// <summary>
    /// Обновление UI
    /// </summary>
    private void UpdateUI()
    {
        currentBall.color = BubbleGraph.instance.GetColorByEnum(currentColor);
        nextBall.color = BubbleGraph.instance.GetColorByEnum(nextColor);
    }

    /// <summary>
    /// Обновление визуального положения линии-траектории (работает как лазерная указка)
    /// </summary>
    private void LineRendererUpdate(bool setDefaults = true)
    {
        Vector2 spawnPoint = bubbleSpawnPoint.position;
        Vector3[] newValues = new Vector3[] { spawnPoint, spawnPoint };
        if (setDefaults)
        {
            lineRenderer.SetPositions(newValues);
            return;
        }
        newValues[1] = Physics2D.Raycast(bubbleSpawnPoint.position, 
            transform.rotation * Vector2.up, float.MaxValue, ~ignoreLayers).point;
        lineRenderer.SetPositions(newValues);
    }
}
