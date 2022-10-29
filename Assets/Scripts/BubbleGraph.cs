using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Возможные цвета пузырей
/// </summary>
public enum BubbleColor
{
    // none - выбирается рандомный цвет
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

    [SerializeField] private int maxGraphDepth = 15;
    [SerializeField] private int initialGraphDepth = 7;
    /// <summary>
    /// Максимальное число пузырей в нечетной строке (четные высчитываются как данное число - 1)
    /// </summary>
    [Range(2, 100)] [SerializeField] private int oddGraphLength = 11;

    [SerializeField] private float horizontalBubbleGap = 0.5f;
    [SerializeField] private float verticalBubbleGap = 0.425f;
    [SerializeField] private GameObject bubblePrefab;
    /// <summary>
    /// Массив с цветами для окраски пузырей (желательно логическое соответствие типу BubbleColor, кроме None)
    /// </summary>
    [SerializeField] private Color[] bubbleColors;

    private int countLeftToGenerate;
    private int generatedLines = 0;
    private bool isNextLineOdd = false;
    private bool shouldUpdateBubblesNow = true;
    private List<Bubble[]> bubbles;
    
    private void Start()
    {
        bubbles = new List<Bubble[]>();
        countLeftToGenerate = maxGraphDepth - initialGraphDepth;
        RandomizeLevel();
        //InvokeRepeating(nameof(RandomizeOneLine), 1f, 1f);
    }

    public void UpdateAllListeners()
    {
        //return;
        if (!shouldUpdateBubblesNow)
        {
            return;
        }

        shouldUpdateBubblesNow = false;
        foreach(Bubble[] b in bubbles)
        {
            if (b[0] == null) 
            { 
                continue;
            }
            b[0].TryGetSomeNeighbors();
        }
        StartCoroutine(UpdateRevoke());
    }

    private IEnumerator UpdateRevoke()
    {
        yield return new WaitForSeconds(UPDATERTIMER);
        shouldUpdateBubblesNow = true;
    }

    /// <summary>
    /// Спавнит одну линию пузырьков сверху остальных
    /// </summary>
    private void RandomizeOneLine()
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
            GenerateOddline(-generatedLines);
        }
        else
        {
            GenerateEvenLine(-generatedLines);
        }
        isNextLineOdd = !isNextLineOdd;
    }

    /// <summary>
    /// Рандомизирует первые initialGraphDepth линий пузырьков
    /// </summary>
    private void RandomizeLevel()
    {
        for (int i = 0; i < initialGraphDepth; i++)
        {
            if ((i & 0x1) == 0x0)
            {
                GenerateOddline(i);
                continue;
            }
            GenerateEvenLine(i);
        }
    }

    /// <summary>
    /// Рандомизирует нечетную линию (максимальное кол-во пузырьков)
    /// </summary>
    /// <param name="yIndex">Номер линии</param>
    private void GenerateOddline(int yIndex)
    {
        for (int j = 0; j < oddGraphLength; j++)
        {
            Bubble bubble = Instantiate(bubblePrefab, transform.position +
                new Vector3(horizontalBubbleGap * j, -verticalBubbleGap * yIndex, 0f),
                Quaternion.identity, transform).AddComponent(typeof(Bubble)) as Bubble;
            bubble.Initialize(j, yIndex);
            bubble.GetComponent<SpriteRenderer>().color = bubbleColors[bubble.Color];
            bubbles.Add(new Bubble[3] { bubble, null, null });
        }
    }

    /// <summary>
    /// Рандомизирует четную линию (максимальное кол-во пузырьков - 1)
    /// </summary>
    /// <param name="yIndex">Номер линии</param>
    private void GenerateEvenLine(int yIndex)
    {
        for (int j = 0; j < oddGraphLength - 1; j++)
        {
            Bubble bubble = Instantiate(bubblePrefab, transform.position +
                new Vector3(horizontalBubbleGap / 2 + horizontalBubbleGap * j, -verticalBubbleGap * yIndex, 0f),
                Quaternion.identity, transform).AddComponent(typeof(Bubble)) as Bubble;
            bubble.Initialize(j, yIndex);
            bubble.GetComponent<SpriteRenderer>().color = bubbleColors[bubble.Color];
            bubbles.Add(new Bubble[3] { bubble, null, null });
        }
    }
}
