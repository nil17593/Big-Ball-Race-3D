using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHoldingCubeController : MonoBehaviour
{
    public enum CubeColor
    {
        Red,
        Yellow,
        Blue,
        Pink,
    }

    public CubeColor cubeColor;

    [Header("Blue Basket Cubes To HoldBalls")]
    public GameObject[] blueCubesToHoldBalls;
    [Header("Yellow Basket Cubes To HoldBalls")]
    public GameObject[] yellowCubesToHoldBalls;
    [Header("Red Basket Cubes To HoldBalls")]
    public GameObject[] redCubesToHoldBalls;
    [Header("Player Basket Cubes To HoldBalls")]
    public GameObject[] playerBasketCubesToHoldBalls;

    public static int blueBallCount;
    public static int redBallCount;
    public static int yellowBallCount;
    public static int playerBallCount;


    public static BallHoldingCubeController Instance;


    private void Start()
    {
        BasketHandler[] baskets = (BasketHandler[])GameObject.FindObjectsOfType(typeof(BasketHandler));

        foreach (BasketHandler basket in baskets)
        {
            int index = (int)basket.BColor;
            if (index==2)//Blue
            {
                
                blueCubesToHoldBalls = basket.cubesToHoldBalls;

            }
            if (index == 1)//Yellow
            {
                yellowCubesToHoldBalls = basket.cubesToHoldBalls;
            }
            if (index == 3)//Pink
            {
                playerBasketCubesToHoldBalls = basket.cubesToHoldBalls;
            }
            if (index == 0)//Red
            {
                redCubesToHoldBalls = basket.cubesToHoldBalls;
            }

        }

        Instance = this;
        blueBallCount = 0;
        redBallCount = 0;
        yellowBallCount = 0;
        playerBallCount = 0;
        DeactivateCubes();
    }

    private void Update()
    {
        BlueBasketCubes();
        RedBasketCubes();
        YellowBasketCubes();
        PlayerBasketCubes();
    }

    void BlueBasketCubes()
    {
        if (blueBallCount == 1)
        {
            blueCubesToHoldBalls[0].SetActive(true);
        }
        if (blueBallCount == 2)
        {
            blueCubesToHoldBalls[1].SetActive(true);
        }
        if (blueBallCount == 3)
        {
            blueCubesToHoldBalls[2].SetActive(true);
        }
    }

    void RedBasketCubes()
    {
        if (redBallCount == 1)
        {
            redCubesToHoldBalls[0].SetActive(true);
        }
        if (redBallCount == 2)
        {
            redCubesToHoldBalls[1].SetActive(true);
        }
        if (redBallCount == 3)
        {
            redCubesToHoldBalls[2].SetActive(true);
        }
    }

    void YellowBasketCubes()
    {
        if (yellowBallCount == 1)
        {
            yellowCubesToHoldBalls[0].SetActive(true);
        }
        if (yellowBallCount == 2)
        {
            yellowCubesToHoldBalls[1].SetActive(true);
        }
        if (yellowBallCount == 3)
        {
            yellowCubesToHoldBalls[2].SetActive(true);
        }
    }

    void PlayerBasketCubes()
    {
        if (playerBallCount == 1)
        {
            playerBasketCubesToHoldBalls[0].SetActive(true);
        }
        if (playerBallCount == 2)
        {
            playerBasketCubesToHoldBalls[1].SetActive(true);
        }
        if (playerBallCount == 3)
        {
            playerBasketCubesToHoldBalls[2].SetActive(true);
        }
    }

    public void DeactivateCubes()
    {
        for(int i = 0; i < 3; i++)
        {
            playerBasketCubesToHoldBalls[i].SetActive(false);
            blueCubesToHoldBalls[i].SetActive(false);
            yellowCubesToHoldBalls[i].SetActive(false);
        }
    }
}
