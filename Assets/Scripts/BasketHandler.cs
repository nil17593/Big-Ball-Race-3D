using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketHandler : MonoBehaviour
{
    public enum BasketColor
    {
        Red,
        Yellow,
        Blue,
        Pink,
    }

    public BasketColor BColor;
    public Transform BallPositionAI;
    public Transform Target;
    public Transform Basket;
    public Transform BallTrowPosition;
    public GameObject[] cubesToHoldBalls;
   
}
