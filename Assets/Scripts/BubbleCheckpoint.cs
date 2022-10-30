using UnityEngine;

/// <summary>
/// Используется в качестве сенсора для "поддерживающих" шаров
/// </summary>
public class BubbleCheckpoint : MonoBehaviour
{
    // Впихнул в serialize, чтобы отслеживать кол-во пересечений в инспекторе
    [SerializeField] private int overlaps = 0;
    private new Rigidbody2D rigidbody;

    /// <summary>
    /// Пересекается ли коллайдер с чем-нибудь
    /// </summary>
    public bool IsOverlapping
    {
        get => overlaps > 0;
    }

    public bool Simulated
    {
        get => rigidbody.simulated;
        set => rigidbody.simulated = value;
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Увеличивает число пересечений с объектами на 1 (кроме объектов того же слоя)
    /// </summary>
    /// <param name="other">Коллайдер, в который вошли</param>
    private void OnTriggerEnter2D(Collider2D other)
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
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }
        overlaps--;
    }
}
