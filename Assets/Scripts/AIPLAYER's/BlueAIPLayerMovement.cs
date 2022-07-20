using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueAIPLayerMovement : MonoBehaviour
{
    [Header("Player Collecting Ball Settings")]
    public Transform ballPositionAI;
    public float ballGapAI;
    private float ballCurrentHeightAI;

    #region Bools
    public bool isPlayerHoldingBall = false;
    public bool isSelectionArea = false;
    #endregion

    [Header("LIST")]
    public List<GameObject> BallsAI = new List<GameObject>();
    private int i = 0;
    private Animator animator;
    private NavMeshAgent agent;
    public Transform target;

    public GameObject[] targets;


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
    #endregion

    public int ballHoldingCountAI = 0;
    public Material ballMaterial;
    Color ballColor = new Color(255f, 252f, 0f, 255f);
    void Start()
    {

        
        
        ballCurrentHeightAI = ballPositionAI.transform.position.y;
        ballPlaceHeightAI = BallPlaceAreaAI.transform.position.y;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        targets = GameObject.FindGameObjectsWithTag("AI_Balls");
        agent.destination = targets[i].transform.position;
        animator.SetBool("AI_Idle", false);
        animator.SetBool("AI_Running", true);

        #region Ball Trowing
        directionCube.transform.eulerAngles = new Vector3(directionCubeInitAngle, 0, 0);
        countTouch = false;
        delayAnimation = true;
        touchTime = 0;
        throwSpeed = minThrowSpeed;
        dir = directionCube.transform.forward;
        #endregion
    }

    private void Update()
    {
        //if (i == targets.Length)
        //{
        //    agent.ResetPath();
        //    agent.destination = target.transform.position;
        //}
        if (ballHoldingCountAI == 3)
        {
            agent.destination = target.position;
        }
        CheckIfPlayerHodingBall();
        ChangeAnimationStateToBallIdle();
        #region Throw Ball
        if (startAIThrow)
        {
            startAIThrow = false;
            ballRigidBody = holdingBall.GetComponent<Rigidbody>();
            ballRigidBody.constraints = RigidbodyConstraints.None;
            ballRigidBody.velocity = dir * 15f;
            animator.SetBool("AI_BaLLIdle", false);
            animator.SetBool("AI_BallThrow", true);

            StartCoroutine(TouchTimer());
        }

    }

    IEnumerator InitiateShoot()
    {
        yield return new WaitForSeconds(animationDelayTime);
        if (delayAnimation == true)
        {
            animator.speed = 0;
        }
    }

    IEnumerator TouchTimer()
    {
        StartCoroutine(InitiateShoot());

        setAngle = true;
        countTouch = false;
        int k = 0;

        float randomThrow = Random.Range(10f, 15f);

        while (k <= projectionDelay)
        {
            touchTime += 0.005f;
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
                touchTime += 0.005f;
                throwSpeed += touchTime * throwFactor;
                Debug.Log(throwSpeed);
                /*if (arrowSpeed >= maxArrowSpeed)
                {
                    break;
                }*/

                //Debug.Log(arrowSpeed);
                //projection.gameObject.SetActive(true);
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


        //projectionOne.gameObject.SetActive(false);


        animator.speed = 1;
        touchTime = 0;
        StartCoroutine(ThrowBall());
    }


    IEnumerator ThrowBall()
    {
        //rb.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(shootTime);


        ballRigidBody.constraints = RigidbodyConstraints.None;
        ballRigidBody.velocity = dir * throwSpeed;
        ballRigidBody.useGravity = true;
        holdingBall.transform.SetParent(null);
        if (BallsAI.Count == 1)
        {
            BallsAI.RemoveAt(0);
        }

        animator.SetBool("AI_BallThrow", false);

        if (BallsAI.Count >= 1)
        {
            animator.SetBool("AI_BaLLIdle", true);
            animator.SetBool("AI_Idle", false);
        }
        else
        {
            animator.SetBool("AI_Idle", true);
            animator.SetBool("AI_BaLLIdle", false);
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
            Debug.Log("AFTER REMOVING " + BallsAI.Count);
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

    //Player Movement

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

    void PlaceBallAtBallPlaceArea()
    {
        if (BallsAI.Count >= 1)
        {
            for (int i = 1; i < BallsAI.Count; i++)
            {
                BallsAI[i].gameObject.tag = "Placed Balls";
                
                Vector3 posi = BallPlaceAreaAI.gameObject.transform.position;
                posi.y = ballPlaceHeightAI;
                BallsAI[i].transform.position = posi;
                BallsAI[i].transform.parent = BallPlaceAreaAI.transform;
                ballPlaceHeightAI += 0.5f;
                Debug.Log("Balls" + BallsAI.Count);
            }
            //Balls.RemoveAt(0);
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (ballHoldingCountAI < 3)
        {
            //other.gameObject.GetComponent<Renderer>().material.color == new Color(255f, 252f, 0f, 255f)
            if (other.gameObject.GetComponent<Renderer>().material.color == new Color(255f, 252f, 0f, 255f))
            {
                ballHoldingCountAI += 1;
                BallsAI.Add(other.gameObject);
                holdingBall = BallsAI[0];
                Vector3 pos = ballPositionAI.transform.position;//temporary vector to hold the ball current pos
                pos.y = ballCurrentHeightAI;//
                targets[i].gameObject.transform.position = pos;//set ball gameobjects position
                targets[i].gameObject.transform.SetParent(ballPositionAI.transform);//set parent of ball gameobject
                ballCurrentHeightAI += ballGapAI;
                targets[i] = null;
                i += 1; //change next target
                if (i < targets.Length)
                {
                    agent.destination = targets[i].transform.position; //go to next target by setting it as the new destination
                }
            }
        }
        else
        {
            agent.ResetPath();
            agent.destination = target.transform.position;
        }
        if (other.gameObject.CompareTag("Ball Throw Area"))
        {
            ballHoldingCountAI = 0;
            agent.isStopped = true;
            isSelectionArea = true;
            animator.SetBool("AI_BaLLIdle", true);
            animator.SetBool("AI_BallRunning", false);
            PlaceBallAtBallPlaceArea();

            if (BallsAI.Count > 0)
            {
                StartCoroutine(WaitForBallThrow());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ball Throw Area"))
        {
            if (BallsAI.Count == 0)
            {
                agent.ResetPath();
                agent.isStopped = false;
                agent.acceleration = 2000f;
                animator.SetBool("AI_Running", true);
                animator.SetBool("AI_Idle", false);
                if (targets != null)
                {
                    if (i >= 0 && i < targets.Length)
                    {
                        agent.destination = targets[i].transform.position;
                    }
                }
                else
                {
                    return;
                }
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
        }
    }

    IEnumerator WaitForBallThrow()
    {
        yield return new WaitForSeconds(2f);
        startAIThrow = true;
    }
}
