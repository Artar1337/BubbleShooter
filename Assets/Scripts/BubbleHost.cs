using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleHost : MonoBehaviour
{
    /// <summary>
    /// Ключ - номер строки, сет - все элементы строки, пара - индекс столбца и вторая пара,
    /// вторая пара - компонент Bubble данного объекта, и list - все соседние к нему Bubble
    /// </summary>
    private Dictionary<int, HashSet<KeyValuePair<int, KeyValuePair<Bubble, List<Bubble>>>>> bubbles;

    public void BubblesRandomize(int lineCount = 5, int oddGraphLength = 11)
    {
        bubbles = new Dictionary<int, HashSet<KeyValuePair<int, KeyValuePair<Bubble, List<Bubble>>>>>();

        for(int i = 0; i < lineCount; i++)
        {
            if ((i & 0x1) == 0x0)
            {
                bubbles.Add(i, FillLine(oddGraphLength));
                continue;
            }
            bubbles.Add(i, FillLine(oddGraphLength - 1));
        }
    }

    /// <summary>
    /// Просчет связей между пузыриками
    /// </summary>
    public void CalculateBubbleConnections()
    {

    }

    /// <summary>
    /// Формирует hashset нужной длины (не заполняет связи между пузырями)
    /// </summary>
    /// <param name="lineLength">Длина строки пузырей</param>
    /// <returns>Hashset нужной длины</returns>
    private HashSet<KeyValuePair<int, KeyValuePair<Bubble, List<Bubble>>>> FillLine(int lineLength)
    {
        var line = new HashSet<KeyValuePair<int, KeyValuePair<Bubble, List<Bubble>>>>();
        for (int j = 0; j < lineLength; j++)
        {
            line.Add(new KeyValuePair<int, KeyValuePair<Bubble, List<Bubble>>>(j, new KeyValuePair<Bubble, List<Bubble>>()));
        }
        return line;
    }
}
