using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StickmanController : MonoBehaviour
{
    private const string BLINKTRIGGER = "Blink";
    private const string HAPPYTRIGGER = "Happy";
    private const string SADTRIGGER = "Sad";

    [SerializeField] private Sprite blink;
    [Range(0f, 1f)][SerializeField] private float blinkChance = 0.25f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Меняет спрайт на моргнувший случайным образом, зависит от blinkChance
    /// </summary>
    public void BlinkRandomly()
    {
        if(Random.Range(0f, 1f) < blinkChance)
        {
            animator.SetTrigger(BLINKTRIGGER);
        }
    }

    /// <summary>
    /// Заставит стикмана улыбнуться на время
    /// </summary>
    public void MakeStickmanHappy()
    {
        animator.SetTrigger(HAPPYTRIGGER);
    }

    /// <summary>
    /// Заставит стикмана погрустнеть навсегда
    /// </summary>
    public void MakeStickmanSad()
    {
        animator.SetTrigger(SADTRIGGER);
    }
}
