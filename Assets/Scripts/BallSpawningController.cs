using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawningController : MonoBehaviour
{
    public Vector3 spawnValues;

    public GameObject blueBall;
    public GameObject yellowBall;
    public GameObject playerBall;
    public static bool isBallSpawned=false;
    public static bool isPlayerBallSpawned= false;

    public AIPlayerMovement playerMovementYellow;
    public AIPlayerMovement playermovementBlue;
    public CharacterMovement characterMovement;

    public static BallSpawningController Instance;

    void Start()
    {
        Instance = this;
        //find player and save player color 
    }


    public void SpawnBalls()
    {
        if (!isBallSpawned)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), 0.5f, Random.Range(-spawnValues.z, spawnValues.z));
                GameObject newBall = Instantiate(yellowBall, spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
                playerMovementYellow.AddTargetsToList(newBall);
            }

        }
        if (!isBallSpawned)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), 0.5f, Random.Range(-spawnValues.z, spawnValues.z));
                GameObject newBall = Instantiate(blueBall, spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
                playermovementBlue.AddTargetsToList(newBall);
            }
            
        }
        isBallSpawned = true;
    }


    public void SpawnPlayerBalls()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), 0.5f, Random.Range(-spawnValues.z, spawnValues.z));
            GameObject newBall = Instantiate(playerBall, spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
            newBall.tag = "Player_Ball";
        }
        isPlayerBallSpawned = true;
    }
}
