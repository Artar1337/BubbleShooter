using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bubble: MonoBehaviour
{
    private const float DETECTIONRADIUS = 3f;
    private const float TIMETOLIVE = 0.3f;

    private BubbleColor bubbleColor;
    private new CircleCollider2D collider;
    private float defaultRadius;
    public bool hadSomeCollisions = false;
    
    private int xIndex = 0;
    private int yIndex = 0;

    public int Color
    {
        get => (int)bubbleColor - 1;
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
        yield return new WaitForSeconds(TIMETOLIVE);
        if (!hadSomeCollisions)
        {
            Destroy(gameObject);
        }
        collider.radius = defaultRadius;
        hadSomeCollisions = false;
    }

    /// <summary>
    /// Инициализация параметров пузыря
    /// </summary>
    /// <param name="xIndex">Индекс колонки пузыря</param>
    /// <param name="yIndex">Индекс строки пузыря</param>
    /// <param name="color">Цвет пузыря</param>
    public void Initialize(int xIndex, int yIndex, BubbleColor color = BubbleColor.None)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
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
        hadSomeCollisions = false;
        collider.radius = DETECTIONRADIUS;
        StartCoroutine(NeighborCheck());
    }
}