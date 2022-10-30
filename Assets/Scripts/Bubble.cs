using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс пузыря, отвечает за просчёты коллизий
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bubble: MonoBehaviour
{
    private const float DETECTIONRADIUS = 3f;
    private const float TIMETOLIVE = 0.3f;

    [SerializeField] private BubbleCheckpoint[] checkpoints;

    private BubbleColor bubbleColor;
    private new CircleCollider2D collider;
    private float defaultRadius;
    private bool hadSomeCollisions = false;

    /// <summary>
    /// Нормализованный цвет (начинается с нуля)
    /// </summary>
    public int Color
    {
        get => (int)bubbleColor - 1;
    }

    public bool HadSomeCollisions 
    {
        get => hadSomeCollisions;
        set => hadSomeCollisions = value;
    }

    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        defaultRadius = collider.radius;
    }

    /// <summary>
    /// Когда происходит касание с шариком другого цвета
    /// </summary>
    /// <param name="collision">Коллизия</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hadSomeCollisions = true;
        if(collision.gameObject.layer != gameObject.layer)
        {
            return;
        }
        collider.radius = DETECTIONRADIUS;
        StartCoroutine(RadiusRevoke());
    }

    /// <summary>
    /// Возврат радиуса к нормальному значению
    /// </summary>
    /// <returns>Ждет TIMETOLIVE</returns>
    private IEnumerator RadiusRevoke()
    {
        yield return new WaitForSeconds(TIMETOLIVE);
        BubbleGraph.instance.UpdateAllListeners();
        Destroy(gameObject);
    }

    /// <summary>
    /// Удаление шаров без коллайдеров с соседями 
    /// </summary>
    /// <returns>Ждет FixedUpdate</returns>
    private IEnumerator NeighborCheck()
    {
        hadSomeCollisions = false;
        // Ждем, чтобы убедиться, что объекты уже уничтожены
        yield return new WaitForSeconds(TIMETOLIVE);
        foreach(BubbleCheckpoint checkpoint in checkpoints)
        {
            if (checkpoint.IsOverlapping)
            {
                hadSomeCollisions = true;
                break;
            }
        }
        Debug.LogWarning(hadSomeCollisions);
        if (!hadSomeCollisions)
        {
            Destroy(gameObject);
        }
        hadSomeCollisions = false;
    }

    /// <summary>
    /// Инициализация параметров пузыря
    /// </summary>
    /// <param name="xIndex">Индекс колонки пузыря</param>
    /// <param name="yIndex">Индекс строки пузыря</param>
    /// <param name="color">Цвет пузыря</param>
    public void Initialize(BubbleColor color = BubbleColor.None)
    {
        bubbleColor = color;
        if(bubbleColor == BubbleColor.None)
        {
            bubbleColor = (BubbleColor)Random.Range(1, System.Enum.GetNames(typeof(BubbleColor)).Length);
        }
        gameObject.layer = LayerMask.NameToLayer(bubbleColor.ToString());
    }

    /// <summary>
    /// Выяснение, есть ли у пузырика соседи
    /// </summary>
    public void TryGetSomeNeighbors()
    {
        if (!hadSomeCollisions)
        {
            return;
        }
        StartCoroutine(NeighborCheck());
    }
}