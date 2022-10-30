using UnityEngine;

/// <summary>
/// Используется в качестве сенсора для "поддерживающих" шаров
/// </summary>
public class BubbleCheckpoint : MonoBehaviour
{
    public int overlaps = 0;

    /// <summary>
    /// Пересекается ли коллайдер с чем-нибудь
    /// </summary>
    public bool IsOverlapping
    {
        get
        {
            return overlaps > 0;
        }
    }

    /// <summary>
    /// Увеличивает число пересечений с объектами на 1 (кроме объектов того же слоя)
    /// </summary>
    /// <param name="other">Коллайдер, в который вошли</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == gameObject.layer)
        {
            return;
        }
        overlaps++;
    }

    /// <summary>
    /// Уменьшает число пересечений с объектами на 1 (кроме объектов того же слоя)
    /// </summary>
    /// <param name="other">Коллайдер, из которого вышли</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }
        overlaps--;
    }
}
