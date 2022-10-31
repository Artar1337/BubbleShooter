using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Возможные цвета пузырей
/// </summary>
public enum BubbleColor
{
    // none - выбирается рандомный цвет в Initialize в классе Bubble
    None = 0,
    Green = 1,
    Red = 2,
    Blue = 3,
    Yellow = 4
}

/// <summary>
/// Используется для визуального построения графа из пузырей
/// </summary>
public class BubbleGraph : MonoBehaviour
{
    #region Singleton
    public static BubbleGraph instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Bubble root instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    private const float UPDATERTIMER = 0.5f;
    
    private const string CHECKPOINTLAYER = "Checkpoint";
    private const string BALLLAYER = "Ball";
    private const string BOUNCYWALLLAYER = "BouncyWall";

    [SerializeField] private int maxGraphDepth = 15;
    [SerializeField] private int initialGraphDepth = 7;
    /// <summary>
    /// Максимальное число пузырей в нечетной строке (четные высчитываются как данное число - 1)
    /// </summary>
    [Range(2, 11)] [SerializeField] private int oddGraphLength = 11;

    [SerializeField] private float horizontalBubbleGap = 0.5f;
    [SerializeField] private float verticalBubbleGap = 0.425f;
    [SerializeField] private GameObject bubblePrefab;
    /// <summary>
    /// Массив с цветами для окраски пузырей (желательно логическое соответствие типу BubbleColor, кроме None)
    /// </summary>
    [SerializeField] private Color[] bubbleColors;
    [SerializeField] private float forceUpdateTimer = 0.25f;

    private int countLeftToGenerate;
    private int generatedLines = 0;
    private bool isNextLineOdd = false;
    private bool shouldUpdateBubblesNow = true;
    private List<Bubble> bubbles;
    private int currentId = 0;

    public GameObject BubblePrefab 
    {
        get => bubblePrefab;
        private set => bubblePrefab = value;
    }
    
    private void Start()
    {
        PhysicsIgnoreInitialize();
        bubbles = new List<Bubble>();
        countLeftToGenerate = maxGraphDepth - initialGraphDepth;
        InitializeLevel(LevelManager.instance.GetCurrentLevel());
        InvokeRepeating(nameof(ForceUpdateAllListeners), forceUpdateTimer, forceUpdateTimer);
    }

    /// <summary>
    /// Используется для того, чтобы нельзя было вызвать UpdateAllListeners несколько раз подряд
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateRevoke()
    {
        yield return new WaitForSeconds(UPDATERTIMER);
        shouldUpdateBubblesNow = true;
    }

    /// <summary>
    /// Спавнит одну линию пузырьков сверху остальных
    /// </summary>
    private void GenerateOneLineAbove()
    {
        if (countLeftToGenerate <= 0)
        {
            return;
        }
        countLeftToGenerate--;
        generatedLines++;
        // Сдвигаем сначала вниз все пузыри, потом уже билдим сверху
        transform.position -= new Vector3(0, verticalBubbleGap, 0);
        if (isNextLineOdd)
        {
            GenerateOddline(-generatedLines, null);
        }
        else
        {
            GenerateEvenLine(-generatedLines, null);
        }
        isNextLineOdd = !isNextLineOdd;
    }

    /// <summary>
    /// Инициализирует первые initialGraphDepth линий пузырьков (последние для файла)
    /// </summary>
    private void InitializeLevel(string level)
    {
        string[] lines = level.Split(System.Environment.NewLine);

        for (int i = 0; i < initialGraphDepth; i++)
        {
            if ((i & 0x1) == 0x0)
            {
                GenerateOddline(i, lines[lines.Length - 2 - initialGraphDepth + i]);
                continue;
            }
            GenerateEvenLine(i, lines[lines.Length - 2 - initialGraphDepth + i]);
        }
    }

    /// <summary>
    /// Создает нечетную линию (максимальное кол-во пузырьков)
    /// </summary>
    /// <param name="yIndex">Номер линии</param>
    /// <param name="lineInfo">Информация о строке</param>
    private void GenerateOddline(int yIndex, string lineInfo)
    {
        for (int j = 0; j < oddGraphLength; j++)
        {
            Bubble bubble = Instantiate(bubblePrefab, transform.position +
                new Vector3(horizontalBubbleGap * j, -verticalBubbleGap * yIndex, 0f),
                Quaternion.identity, transform).GetComponent<Bubble>();
            bubble.Initialize(false, Vector2.zero, CharNumberToBubbleColor(lineInfo[j]));
        }
    }

    /// <summary>
    /// Создает четную линию (максимальное кол-во пузырьков - 1)
    /// </summary>
    /// <param name="yIndex">Номер линии</param>
    /// /// <param name="lineInfo">Информация о строке</param>
    private void GenerateEvenLine(int yIndex, string lineInfo)
    {
        for (int j = 0; j < oddGraphLength - 1; j++)
        {
            Bubble bubble = Instantiate(bubblePrefab, transform.position +
                new Vector3(horizontalBubbleGap / 2 + horizontalBubbleGap * j, -verticalBubbleGap * yIndex, 0f),
                Quaternion.identity, transform).GetComponent<Bubble>();
            bubble.Initialize(false, Vector2.zero, CharNumberToBubbleColor(lineInfo[j]));
        }
    }

    /// <summary>
    /// Конвертирует char в BubbleColor с учетом того, что отсчет начинаем с символа '0'
    /// </summary>
    /// <param name="ch">Входной символ</param>
    /// <returns>Выходной цвет</returns>
    private BubbleColor CharNumberToBubbleColor(char ch)
    {
        return (BubbleColor)(ch - '0');
    }
    
    /// <summary>
    /// Изначальный игнор коллизий снаряда и чекпоинтов
    /// </summary>
    private void PhysicsIgnoreInitialize()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CHECKPOINTLAYER), LayerMask.NameToLayer(BALLLAYER));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(BALLLAYER), LayerMask.NameToLayer(BALLLAYER));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CHECKPOINTLAYER), LayerMask.NameToLayer(BOUNCYWALLLAYER));
    }

    /// <summary>
    /// Используется для просчета возможности уничтожения висящих в воздухе шаров
    /// </summary>
    public void UpdateAllListeners()
    {
        if (!shouldUpdateBubblesNow)
        {
            return;
        }
        shouldUpdateBubblesNow = false;
        foreach(Bubble b in bubbles)
        {
            if (b == null) 
            { 
                continue;
            }
            b.TryGetSomeNeighbors();
        }
        StartCoroutine(UpdateRevoke());
    }

    /// <summary>
    /// Возвращает цвет по его enum
    /// </summary>
    /// <param name="color">Цвет в enum</param>
    /// <returns>Цвет в Color (ARGB)</returns>
    public Color GetColorByEnum(BubbleColor color)
    {
        if (color == BubbleColor.None)
        {
            return Color.white;
        }
        return bubbleColors[(int)color - 1];
    }

    /// <summary>
    /// Принудительное обновление шариков 
    /// (вызывается по таймеру как фикс от зависших в воздухе для каждого шара, кроме шаров со слоем Ball)
    /// </summary>
    private void ForceUpdateAllListeners()
    {
        foreach (Bubble b in bubbles)
        {
            if (b == null)
            {
                continue;
            }
            if(b.gameObject.layer != LayerMask.NameToLayer(BALLLAYER))
            {
                b.ForceGetSomeNeighbors();
            }
        }
    }

    /// <summary>
    /// Добавляет пузырь в список и красит его в нужный цвет
    /// </summary>
    /// <param name="bubble">Пузырь для добавления</param>
    public void AddAndColorBubble(Bubble bubble)
    {
        bubble.GetComponent<SpriteRenderer>().color = bubbleColors[bubble.Color];
        bubbles.Add(bubble);
    }

    /// <summary>
    /// Возврат следующего уникального ID шарика
    /// </summary>
    /// <returns>ID пузыря</returns>
    public int GetNextId()
    {
        return currentId++;
    }
}
