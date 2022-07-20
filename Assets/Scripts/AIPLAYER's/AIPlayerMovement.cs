using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIPlayerMovement : MonoBehaviour
{
    //[Header("Prefix Color")]
    public enum AIBallColor
    {
        Red,
        Yellow,
        Blue,
        Pink,
    }
    public AIBallColor AIColor;

    [Header("Player Collecting Ball Settings")]
    public Transform ballPositionAI;
    public float ballGapAI;
    private float ballCurrentHeightAI;

    #region Bools
    public bool isPlayerHoldingBall = false;
    public bool isSelectionArea = false;
    public bool exitBallThrowArea = false;
    #endregion

    [Header("LIST")]
    public List<GameObject> BallsAI = new List<GameObject>();
    public int i = 0;
    private Animator animator;
    private NavMeshAgent agent;
    public Transform target;

    //public GameObject[] targets;
    public List<GameObject> targets = new List<GameObject>();

    [Header("Ball Place Settings At Launchpad")]
    public Transform BallPlaceAreaAI;

    [Header("Add float values to set height of balls")]
    private float ballPlaceHeightAI;
    public float ballGapAtBallPlaceAreaAI;

    #region Ball Throw
    public GameObject directionCube;
    public float directionCubeInitAngle;

    private GameObject holdingBall;
    private bool countTouch;
    private bool setAngle;
    private bool delayAnimation;
    private bool startAIThrow = false;


    public float projectionDelay;
    private float touchTime;
    private float throwSpeed;
    public float minThrowSpeed;
    public float throwFactor;
    public float maxThrowSpeed;
    public float shootTime;
    private Vector3 dir;

    private Rigidbody ballRigidBody;
    public float animationDelayTime;


    public GameObject projection;
    public float randomThrowValueOne;
    public float randomThrowValueTwo;
    public float randomThrowValuethree;
    #endregion

    public int ballHoldingCountAI = 0;
    public Transform Basket_Main;

    public Text playerName;

    public bool callFunction = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        BasketHandler[] baskets = (BasketHandler[])GameObject.FindObjectsOfType(typeof(BasketHandler));

        foreach (BasketHandler basket in baskets)
        {
            if ((int)AIColor == (int)basket.BColor)
            {
                BallPlaceAreaAI = basket.BallPositionAI;
                target = basket.Target;
                Basket_Main = basket.Basket;
            }
        }

        StartCoroutine(StartAddingTargetsToList());

        ballCurrentHeightAI = ballPositionAI.transform.position.y;
        ballPlaceHeightAI = BallPlaceAreaAI.transform.position.y;
        //animator.SetBool("AI_Running", false);
        if (agent.speed > 0f)
        {
            animator.SetBool("AI_Running", true);
            animator.SetBool("AI_Idle", false);
            //animator.SetBool("AI_Running", false);
        }
        else
        {
            animator.SetBool("AI_Running", false);
            animator.SetBool("AI_Idle", true);
        }
        agent.destination = targets[i].transform.position;
     
        #region Ball Throwing
        directionCube.transform.eulerAngles = new Vector3(directionCubeInitAngle, 0, 0);
        countTouch = false;
        delayAnimation = true;
        touchTime = 0;
        throwSpeed = minThrowSpeed;
        dir = directionCube.transform.forward;
        #endregion
    }

    //public void SetPlayerName(TextMesh name)
    //{
    //    playerName = name;
    //    playerName.text = playerName.text;
    //}

    public string GetPlayerName()
    {
        return playerName.text;
    }

    void AddTargets()
    {
        BallHandler[] balls = (BallHandler[])GameObject.FindObjectsOfType(typeof(BallHandler));
        foreach (BallHandler ball in balls)
        {
            if ((int)ball.ballColor == (int)AIColor && callFunction)
            {
                targets.Add(ball.gameObject);
            }
        }
        callFunction = false;
    }

    IEnumerator StartAddingTargetsToList()
    {
        while (true)
        {
            AddTargets();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        if (ballHoldingCountAI == 5)
        {
            agent.ResetPath();
            agent.destination = target.position;
        }


        CheckIfPlayerHodingBall();

        ChangeAnimationStateToBallIdle();


        #region Throw Ball
        if (startAIThrow)
        {
            startAIThrow = false;
            ballRigidBody = holdingBall.GetComponent<Rigidbody>();

            StartCoroutine(TouchTimer());
        }

    }

    IEnumerator TouchTimer()
    {
        animator.SetBool("AI_Idle", false);
        animator.SetBool("AI_BaLLIdle", false);


        animator.Play("Character_Ball_Throw_Start");

        setAngle = true;
        countTouch = false;
        int k = 0;

        float[] a = new float[] { randomThrowValueOne, randomThrowValueTwo, randomThrowValuethree };

        float randomThrow = a[Random.Range(0, 2)];

        projection.gameObject.SetActive(true);
        while (k <= projectionDelay)
        {
            touchTime += 0.0001f;
            throwSpeed += touchTime * throwFactor;
            Debug.Log(throwSpeed);
            k++;
            yield return new WaitForSeconds(0.1f);

            if (throwSpeed >= randomThrow)
            {
                setAngle = false;
            }

            if (setAngle == false)
            {
                delayAnimation = false;
                goto abc;
            }
        }


        while (setAngle == true)
        {

            if (setAngle == true)
            {
                touchTime += 0.001f;
                throwSpeed += touchTime * throwFactor;
                k++;
            }

            else
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);


            if (throwSpeed >= randomThrow)
            {
                setAngle = false;
            }
        }

    abc:

        projection.gameObject.SetActive(false);
        animator.speed = 1;
        touchTime = 0;




        StartCoroutine(ThrowBall());
    }


    IEnumerator ThrowBall()
    {

        yield return new WaitForSeconds(shootTime);

        ballRigidBody.constraints = RigidbodyConstraints.None;

        animator.SetBool("AI_BaLLIdle", false);
        animator.Play("Character_Ball_Throw_End");

        ballRigidBody.velocity = dir * throwSpeed;
        ballRigidBody.useGravity = true;
        holdingBall.transform.SetParent(null);

        holdingBall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        SphereCollider sphereCollider = holdingBall.transform.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = false;

        if (BallsAI.Count == 1)
        {
            BallsAI.RemoveAt(0);
        }


        if (BallsAI.Count >= 1)
        {
            animator.SetBool("AI_BallThrow_End", false);
            animator.SetBool("AI_BaLLIdle", true);
        }
        else
        {
            animator.SetBool("AI_BallThrow_End", false);
            animator.SetBool("AI_Running", true);
        }
        throwSpeed = minThrowSpeed;
        StartCoroutine(StartCountTouch());
    }


    IEnumerator StartCountTouch()
    {
        yield return new WaitForSeconds(0.5f);
        countTouch = true;
        delayAnimation = true;
        
        if (!(BallsAI.Count <= 0))
        {
            BallsAI[BallsAI.Count - 1].gameObject.transform.position = ballPositionAI.transform.position;
            BallsAI[BallsAI.Count - 1].gameObject.transform.parent = ballPositionAI.transform;
            holdingBall = BallsAI[BallsAI.Count - 1];
            BallsAI.RemoveAt(BallsAI.Count - 1);
        }
        if (BallsAI.Count > 0)
        {
            startAIThrow = true;
        }
        
    }


    #endregion

    void ChangeAnimationStateToBallIdle()
    {
        if (isPlayerHoldingBall)
        {
            if (isSelectionArea)
            {
                animator.SetBool("AI_BaLLIdle", true);
                animator.SetBool("AI_BallRunning", false);

            }
            else
            {
                animator.SetBool("AI_BallRunning", true);
                animator.SetBool("AI_Running", false);
            }

        }
    }

    //place balls to the ball place area
    void PlaceBallAtBallPlaceArea()
    {
        if (BallsAI.Count >= 1)
        {
            for (int i = 1; i < BallsAI.Count; i++)
            {
                BallsAI[0].gameObject.tag = "Placed Balls";
                BallsAI[i].gameObject.tag = "Placed Balls";
                Vector3 posi = BallPlaceAreaAI.gameObject.transform.position;
                posi.y = ballPlaceHeightAI;
                BallsAI[i].transform.position = posi;
                BallsAI[i].transform.parent = BallPlaceAreaAI.transform;
                ballPlaceHeightAI += 0.5f;
            }
        }
    }

    private void CheckIfPlayerHodingBall()
    {
        if (BallsAI.Count > 0)
        {
            isPlayerHoldingBall = true;
        }
        else
        {
            isPlayerHoldingBall = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ballHoldingCountAI < collision.gameObject.GetComponent<CharacterMovement>().holdingBallCount)
            {
                StartCoroutine(FallAndGetup());
                for (int j = 0; j <= ballHoldingCountAI; j++)
                {
                    if (BallsAI.Count > 0)
                    {
                        BallsAI[BallsAI.Count - 1].gameObject.transform.SetParent(null);
                        BallsAI[BallsAI.Count - 1].gameObject.GetComponent<BallHandler>().UnsetSetRBConstraints();
                        targets.Add(BallsAI[BallsAI.Count - 1]);
                        BallsAI.RemoveAt(BallsAI.Count - 1);
                        if (i > 0)
                        {
                            i -= 1;
                        }
                        else
                        {
                            i = 0;
                        }
                    }
                }
                ballHoldingCountAI = 0;
                ballCurrentHeightAI = ballPositionAI.transform.position.y;
            }
        }
        else if (collision.gameObject.CompareTag("Blue_Player") || collision.gameObject.CompareTag("Yellow_Player"))
        {
            if (ballHoldingCountAI < collision.gameObject.GetComponent<AIPlayerMovement>().ballHoldingCountAI)
            {
                StartCoroutine(FallAndGetup());
                for (int j = 0; j <= ballHoldingCountAI; j++)
                {
                    if (BallsAI.Count > 0)
                    {
                        BallsAI[BallsAI.Count - 1].gameObject.transform.SetParent(null);
                        BallsAI[BallsAI.Count - 1].gameObject.GetComponent<BallHandler>().UnsetSetRBConstraints();
                        targets.Add(BallsAI[BallsAI.Count - 1]);
                        BallsAI.RemoveAt(BallsAI.Count - 1);
                        if (i > 0)
                        {
                            i -= 1;
                        }
                        else
                        {
                            i = 0;
                        }
                    }
                }
                ballHoldingCountAI = 0;
                ballCurrentHeightAI = ballPositionAI.transform.position.y;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ballHoldingCountAI < 5)
        {
            if (other.gameObject == targets[i].gameObject)
            {
                ballHoldingCountAI += 1;
                BallsAI.Add(other.gameObject);
                holdingBall = BallsAI[0];
                Vector3 pos = ballPositionAI.transform.position;//temporary vector to hold the ball current pos
                pos.y = ballCurrentHeightAI;//
                targets[i].gameObject.transform.position = pos;//set ball gameobjects position
                targets[i].gameObject.transform.SetParent(ballPositionAI.transform);//set parent of ball gameobject
                ballCurrentHeightAI += ballGapAI;
                targets.RemoveAt(i);
                i += 1; //change next target               
            }
            if (i < targets.Count)
            {
                agent.SetDestination(targets[i].transform.position); //go to next target by setting it as the new destination
            }
            else
            {
                return;
            }
        }
        else
        {
            agent.ResetPath();
            agent.SetDestination(target.transform.position);
        }

        if (other.gameObject.CompareTag("Ball Throw Area"))
        {
            StartCoroutine(EnableSelectionArea());
        }
    }

    IEnumerator EnableSelectionArea()
    {
        yield return new WaitForSeconds(0.5f);

        agent.isStopped = true;
        isSelectionArea = true;

        PlaceBallAtBallPlaceArea();
        transform.LookAt(Basket_Main.position);

        if (BallsAI.Count > 0)
        {
            StartCoroutine(WaitForBallThrow());
        }
        if (i == targets.Count && !BallSpawningController.isBallSpawned)
        {
            BallSpawningController.Instance.SpawnBalls();
            BallSpawningController.isBallSpawned = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ball Throw Area"))
        {
            if (BallsAI.Count == 0)
            {
                StartCoroutine(WaitTillBallReachesBasket());
                if (exitBallThrowArea)
                {
                    ballHoldingCountAI = 0;
                    agent.ResetPath();
                    agent.isStopped = false;
                    agent.acceleration = 2000f;
                    animator.SetBool("AI_Running", true);
                    animator.SetBool("AI_Idle", false);
                    if (targets != null)
                    {
                        if (i >= 0 && i < targets.Count)
                        {
                            agent.SetDestination(targets[i].transform.position);
                        }
                    }                  
                }
               
            }
            exitBallThrowArea = false;
            if (targets.Count == 0)
            {
                agent.isStopped = true;
                animator.SetBool("AI_Idle", true);
                animator.SetBool("AI_Running", false);
                animator.SetBool("AI_BaLLIdle", false);
                animator.SetBool("AI_BallRunning", false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball Throw Area"))
        {
            isSelectionArea = false;
            startAIThrow = false;
            ballCurrentHeightAI = ballPositionAI.transform.position.y;
            ballPlaceHeightAI = BallPlaceAreaAI.transform.position.y;
            animator.SetBool("AI_Running", true);
            animator.SetBool("AI_BaLLIdle", false);
            animator.SetBool("AI_BallThrow", false);
        }
    }

    IEnumerator FallAndGetup()
    {
        agent.isStopped = true;
      //  animator.Play("Character_Ball_Idle");
      //  yield return new WaitForSeconds(1f);
        animator.Play("Falling");
      //  yield return new WaitForSeconds(1.05f);
       // animator.Play("Getting_up");
       yield return new WaitForSeconds(1f);
        agent.isStopped = false;
        animator.Play("Running");
    }

    //wait before start throwing ball
    IEnumerator WaitForBallThrow()
    {
        yield return new WaitForSeconds(2f);
        startAIThrow = true;
    }

    IEnumerator WaitTillBallReachesBasket()
    {
        animator.Play("Idle 0");
        yield return new WaitForSeconds(4f);
        animator.Play("Running");
        exitBallThrowArea = true;    
    }

    public Vector3 ReturnDir()
    {
        return dir;
    }

    public float ReturnThrowSpeed()
    {
        return throwSpeed;
    }

    public void AddTargetsToList(GameObject ball)
    {
        targets.Add(ball);
    }
}