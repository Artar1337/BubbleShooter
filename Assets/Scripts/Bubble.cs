using System.Collections;
using UnityEngine;

/// <summary>
/// Базовый класс пузыря, отвечает за просчёты коллизий
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bubble: MonoBehaviour
{
    private const float DETECTIONRADIUS = 3f;
    private const float TIMETOLIVE = 0.3f;
    private const float TIMETOLIVEAFTERDISABLE = 3f;
    private const string BOUNCYWALLLAYER = "BouncyWall";
    private const string STICKYWALLLAYER = "StickyWall";
    private const string DESTROYWALLLAYER = "DestroyBorder";
    private const string BALLLAYER = "Ball";

    [SerializeField] private BubbleCheckpoint[] checkpoints;
    [SerializeField] private float speed = 100f;

    private BubbleColor bubbleColor;
    private new CircleCollider2D collider;
    private float defaultRadius;
    private bool hadSomeCollisions = false;
    private Vector2 direction;
    private new Rigidbody2D rigidbody;
    private int id;

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

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        defaultRadius = collider.radius;
        rigidbody = GetComponent<Rigidbody2D>();
        id = BubbleGraph.instance.GetNextId();
    }

    /// <summary>
    /// Отражение пузырька при попадании в стену (в том числе и в верхнюю, так надо по тз). 
    /// При этом пузыри - НЕ СНАРЯДЫ к той же стенке всё равно прилипают
    /// </summary>
    /// <param name="collision">Коллайдер, с которым столкнулись</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(BOUNCYWALLLAYER) ||
            collision.gameObject.layer == LayerMask.NameToLayer(STICKYWALLLAYER))
        {
            direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
            if (rigidbody == null)
            {
                return;
            }
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0;
            rigidbody.AddForce(direction * speed, ForceMode2D.Force);
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer(DESTROYWALLLAYER))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Когда происходит касание с шариком другого цвета
    /// </summary>
    /// <param name="collision">Коллизия</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Первый раз попали в триггер другого шарика
        if (!collider.isTrigger)
        {
            collider.isTrigger = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0;
            gameObject.layer = LayerMask.NameToLayer(bubbleColor.ToString());
            foreach (BubbleCheckpoint bubbleCheckpoint in checkpoints)
            {
                bubbleCheckpoint.Simulated = true;
            }
        }

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
        DestroyBall();
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
        if (!hadSomeCollisions)
        {
            DestroyBall();
        }
        hadSomeCollisions = false;
    }

    /// <summary>
    /// Делает объект подверженным гравитации, после чего уничтожает через некоторое время
    /// </summary>
    private void DestroyBall()
    {
        StickmanController.instance.MakeStickmanHappy();
        foreach (BubbleCheckpoint bubbleCheckpoint in checkpoints)
        {
            bubbleCheckpoint.Simulated = false;
        }
        collider.enabled = false;
        rigidbody.gravityScale = 1f;
        StartCoroutine(DestroyCoroutine());
    }

    /// <summary>
    /// Уничтожает объект после TIMETOLIVEAFTERDISABLE секунд
    /// </summary>
    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(TIMETOLIVEAFTERDISABLE);
        Destroy(gameObject);
    }

    /// <summary>
    /// Инициализация параметров пузыря, запуск с определенной скоростью, если это снаряд
    /// </summary>
    /// <param name="xIndex">Индекс колонки пузыря</param>
    /// <param name="yIndex">Индекс строки пузыря</param>
    /// <param name="color">Цвет пузыря</param>
    public void Initialize(bool isProjectile, Vector2 projectileDirection, BubbleColor color = BubbleColor.None)
    {
        bubbleColor = color;
        if(bubbleColor == BubbleColor.None)
        {
            bubbleColor = (BubbleColor)Random.Range(1, System.Enum.GetNames(typeof(BubbleColor)).Length);
        }
        gameObject.layer = LayerMask.NameToLayer(bubbleColor.ToString());
        if (isProjectile)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            collider.isTrigger = false;
            foreach(BubbleCheckpoint bubbleCheckpoint in checkpoints)
            {
                bubbleCheckpoint.Simulated = false;
            }
            direction = projectileDirection;
            rigidbody.AddForce(direction * speed, ForceMode2D.Force);
            gameObject.layer = LayerMask.NameToLayer(BALLLAYER);
        }
        BubbleGraph.instance.AddAndColorBubble(this);
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

    /// <summary>
    /// Принудительное выяснение наличия соседей у пузырька
    /// </summary>
    public void ForceGetSomeNeighbors()
    {
        StartCoroutine(NeighborCheck());
    }
}