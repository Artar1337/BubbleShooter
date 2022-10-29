using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble: MonoBehaviour
{
    private bool isAlive = true;
    private BubbleColor bubbleColor;    
    
    private int xIndex = 0;
    private int yIndex = 0;

    public bool IsAlive
    {
        get => isAlive;
        set => isAlive = value;
    }

    public int Color
    {
        get => (int)bubbleColor;
    }

    public void Initialize(int xIndex, int yIndex, BubbleColor color = BubbleColor.None)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        bubbleColor = color;
        if(bubbleColor == BubbleColor.None)
        {
            bubbleColor = (BubbleColor)(Random.Range(1, sizeof(BubbleColor)));
        }
    }

    public bool IsNeighborTo(Bubble bubble)
    {
        int x = bubble.xIndex;
        int y = bubble.yIndex;
        if((yIndex & 0x1) == 0x0)
        {
            return (x == xIndex - 1 && y == yIndex - 1) ||
                (x == xIndex && y == yIndex - 1) ||
                (x == xIndex - 1 && y == yIndex) ||
                (x == xIndex + 1 && y == yIndex) ||
                (x == xIndex - 1 && y == yIndex + 1) ||
                (x == xIndex && y == yIndex + 1);
        }
        return (x == xIndex && y == yIndex - 1) ||
            (x == xIndex + 1 && y == yIndex - 1) ||
            (x == xIndex - 1 && y == yIndex) ||
            (x == xIndex + 1 && y == yIndex) ||
            (x == xIndex + 1 && y == yIndex + 1) ||
            (x == xIndex && y == yIndex + 1);
    }
}