using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер для пушки, отвечает за стрельбу, поворот, отрисовку линии
/// </summary>
public class CannonController : MonoBehaviour
{
    private const string MOUSEAXIS = "Fire1";
    private const string BOUNCYWALLLAYER = "BouncyWall";
    private const string STICKYWALLLAYER = "StickyWall";
    private const float DEG90 = 90f;
    private const int MAXRECURSIONSTEP = 4;

    [SerializeField] private Camera mainCam;
    [SerializeField] private float degreeBorder = 60f;
    [SerializeField] private Transform bubbleSpawnRoot;
    [SerializeField] private Transform bubbleSpawnPoint;
    [SerializeField] private Image currentBall;
    [SerializeField] private Image nextBall;
    [SerializeField] private LayerMask ignoreLayers;

    private bool cannonIsMoving = false;
    private LineRenderer lineRenderer;
    private BubbleColor currentColor;
    private BubbleColor nextColor;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentColor = GenerateColor();
        nextColor = GenerateColor();
        UpdateUI();
    }

    /// <summary>
    /// Отвечает за INPUT, поворот пушки, выстрел и т.п.
    /// </summary>
    private void Update()
    {
        // Для Android
        // TODO: весь код для anroid input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("Touch Position : " + touch.position);
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
            Shoot();
            transform.rotation = Quaternion.identity;
            currentColor = nextColor;
            nextColor = GenerateColor();
            LineRendererUpdate();
            UpdateUI();
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
    /// Обновление визуального положения линии-траектории
    /// </summary>
    private void LineRendererUpdate(bool setDefaults = true)
    {
        Vector3[] newValues = new Vector3[] {
            lineRenderer.GetPosition(0),
            bubbleSpawnPoint.position,
            bubbleSpawnPoint.position,
            bubbleSpawnPoint.position,
            bubbleSpawnPoint.position
        };

        if (setDefaults)
        {
            lineRenderer.SetPositions(newValues);
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(bubbleSpawnPoint.position, bubbleSpawnPoint.up, 
            float.MaxValue);
        newValues = ReflectRay(hit, bubbleSpawnPoint.up, newValues);
        lineRenderer.SetPositions(newValues);
    }

    /// <summary>
    /// Подсчет траектории выстрела после n-ного рикошета
    /// </summary>
    /// <param name="hit">Объект, которого пузырь коснется последним</param>
    /// <param name="direction">Направление полёта пузыря</param>
    /// <param name="poins">Точки соприкосновения</param>
    /// <param name="recursionStep">Текущий шаг рекурсии</param>
    /// <returns>true, если пуля попадёт по игроку (иначе false)</returns>
    private Vector3[] ReflectRay(RaycastHit2D hit, Vector2 direction, Vector3[] points, int recursionStep = 1)
    {
        if (hit.collider == null || recursionStep > MAXRECURSIONSTEP)
        {
            return points;
        }
        // Выстрел по препятствию - отражение луча
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer(STICKYWALLLAYER) ||
            hit.collider.gameObject.layer == LayerMask.NameToLayer(BOUNCYWALLLAYER))
        {
            points[recursionStep] = hit.point;
            direction = Vector2.Reflect(direction, hit.normal);
            hit = Physics2D.Raycast(hit.point, direction, float.MaxValue);
            return ReflectRay(hit, direction, points, recursionStep + 1);
        }
        // Выстрел по любому шару/границе уничтожения - можно дальше не отражать
        for(int i = recursionStep; i < MAXRECURSIONSTEP; i++)
        {
            points[recursionStep] = hit.point;
        }
        return points;
    }
}
