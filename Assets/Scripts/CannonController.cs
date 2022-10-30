using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private const string MOUSEAXIS = "Fire1";
    private const float DEG90 = 90f;

    [SerializeField] private Camera mainCam;
    [SerializeField] private float degreeBorder = 60f;
    [SerializeField] private Transform bubbleSpawnRoot;
    [SerializeField] private Transform bubbleSpawnPoint;

    private bool cannonIsMoving = false;

    /// <summary>
    /// Отвечает за INPUT, поворот пушки, выстрел и т.п.
    /// </summary>
    private void Update()
    {
        // Для Android
        // TODO: весь код для anroid input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("Touch Position : " + touch.position);
            cannonIsMoving = true;
            return;
        }
        // Для ПК
        if (Input.GetAxis(MOUSEAXIS) > 0)
        {
            Vector3 diff = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z)) - 
                transform.position;
            diff.Normalize();
            transform.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Clamp(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - DEG90, -degreeBorder, degreeBorder));
            cannonIsMoving = true;
            return;
        }
        if (cannonIsMoving)
        {
            cannonIsMoving = false;
            Shoot();
        }
        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Стреляет пузырьком - пушечным шаром в текущем направлении поворота пушки
    /// </summary>
    private void Shoot()
    {
        Bubble bubble = Instantiate(BubbleGraph.instance.BubblePrefab, bubbleSpawnPoint.position, Quaternion.identity, bubbleSpawnRoot).
            GetComponent<Bubble>();
        bubble.Initialize(true, transform.rotation * Vector2.up);
    }
}
