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
    
    private void Start()
    {
        for(int i = 0; i < maxGraphDepth; i++)
        {
            if ((i & 0x1) == 0x0)
            {
                for(int j = 0; j < oddGraphLength; j++)
                {
                    Instantiate(bubblePrefab, transform.position +
                        new Vector3(horizontalBubbleGap * j, -verticalBubbleGap * i, 0f),
                        Quaternion.identity, transform).GetComponent<SpriteRenderer>().color = bubbleColors[j % 4];
                }
                continue;
            }
            for (int j = 0; j < oddGraphLength - 1; j++)
            {
                Instantiate(bubblePrefab, transform.position +
                    new Vector3(horizontalBubbleGap / 2 + horizontalBubbleGap * j, -verticalBubbleGap * i, 0f),
                    Quaternion.identity, transform).GetComponent<SpriteRenderer>().color = bubbleColors[j % 4];
            }
        }
    }

    private void ConstructGraphFromFile()
    {

    }
}
