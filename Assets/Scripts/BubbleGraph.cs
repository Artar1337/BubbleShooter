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
    
    private void Start()
    {
        countLeftToGenerate = maxGraphDepth - initialGraphDepth;
        RandomizeLevel();
        InvokeRepeating(nameof(RandomizeOneLine), 1f, 1f);
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
        // Сдвигаем сначала все пузыри, потом уже билдим сверху
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
        }
    }
}
