using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollector : MonoBehaviour
{
    public enum BallCollectorType
    {
        Blue,
        Red,
        Yellow,
        Pink,
    }

    public BallCollectorType ballCollectorType;
    public static bool yellowWinner = false;
    public static bool bluewWinner = false;
    public static bool redWinner = false;
    public static bool playerWinner = false;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Placed Balls"))
        {
            if (ballCollectorType == BallCollectorType.Blue)
            {
                BallHoldingCubeController.blueBallCount += 1;
                if(BallHoldingCubeController.blueBallCount == 4)
                {
                    bluewWinner = true;
                    UIManager.Instance.TurnOnWinPanel();
                }
            }
            if (ballCollectorType == BallCollectorType.Red)
            {
                BallHoldingCubeController.redBallCount += 1;
                if (BallHoldingCubeController.redBallCount == 4)
                {
                    redWinner = true;
                    UIManager.Instance.TurnOnWinPanel();
                }
            }
            if (ballCollectorType == BallCollectorType.Yellow)
            {
                BallHoldingCubeController.yellowBallCount += 1;
                if (BallHoldingCubeController.yellowBallCount == 4)
                {
                    yellowWinner = true;
                    UIManager.Instance.TurnOnWinPanel();
                }
            }
            if (ballCollectorType == BallCollectorType.Pink)
            {
                BallHoldingCubeController.playerBallCount += 1;
                if (BallHoldingCubeController.playerBallCount == 4)
                {
                    playerWinner = true;
                    UIManager.Instance.TurnOnWinPanel();
                }
            }
        }
    }
}
