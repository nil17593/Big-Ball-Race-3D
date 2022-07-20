using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{   
    public enum MainPlayerBallColor
    {
        Red,
        Yellow,
        Blue,
        Pink,
    }
    public MainPlayerBallColor PlayerColor;

    #region Character Settings
    [Header("Character")]
    [SerializeField]
    private float movementSpeed = 15f;
    [SerializeField]
    private Vector3 rotationSpeed = new Vector3(0, 40, 0);
    [SerializeField]
    private Joystick fixedJoystick;
    private Rigidbody rb;
    private Animator animator;
    public Text playerName;
    #endregion

    [Header("Player Collecting Ball Settings")]

    public GameObject ballPosition;
    public float ballGap;
    private float ballCurrentHeight;

    #region Bools
    public bool ballHold;
    public bool isBallsPlaced = false;
    public bool callFunction = true;
    public bool isThrowingArea = false;
    private bool joystickClick = false;
    private bool forward;
    public bool isReachedToThrowArea = false;
    public bool useJoystick = true;
    private bool makeIdle = true;
    #endregion

    [Header("LIST")]
    public List<GameObject> Balls = new List<GameObject>();
    public List<GameObject> targetBalls = new List<GameObject>();
    private int i = 0;
    private int j = 0;

    [Header("Ball Place Settings At Launchpad")]
    public Transform ballThrowArea;
    public Transform BallPlaceArea;
    public Transform Basket;
    public Transform ballThrowPos;

    [Header("Add float values to set height of balls")]
    private float ballPlaceHeight;
    public float ballGapAtBallPlaceArea;

    #region Ball Throwing
    public GameObject directionCube;

    public float directionCubeInitAngle;

    private GameObject holdingBall;
    private bool countTouch;
    private bool setAngle;
    private bool delayAnimation;

    public float projectionDelay;
    private float touchTime;
    private float throwSpeed;
    public float minThrowSpeed;
    public float throwFactor;
    public float maxThrowSpeed;
    public float shootTime;
    private Vector3 dir;
    public float maximumProjectionFactor;

    private Rigidbody ballRigidBody;
    public float animationDelayTime;

    public GameObject projection;

    #endregion

    public bool isPlayerHoldingBall = false;
    public int holdingBallCount = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        animator.Play("Idle");
        StartCoroutine(StartAddingTargetsToList());
        BasketHandler[] baskets = (BasketHandler[])GameObject.FindObjectsOfType(typeof(BasketHandler));

        foreach (BasketHandler basket in baskets)
        {
            if ((int)PlayerColor == (int)basket.BColor)
            {
                BallPlaceArea = basket.BallPositionAI;
                ballThrowArea = basket.Target;
                Basket = basket.Basket;
                ballThrowPos = basket.BallTrowPosition;
            }
        }
        ballCurrentHeight = ballPosition.transform.position.y;
        ballPlaceHeight = BallPlaceArea.transform.position.y;
        ballHold = false;

        #region Ball Throwing
        directionCube.transform.eulerAngles = new Vector3(directionCubeInitAngle, 0, 0);
        countTouch = false;
        delayAnimation = true;
        touchTime = 0;
        throwSpeed = minThrowSpeed;
        dir = directionCube.transform.forward;
        #endregion
    }

    private void CheckIfPlayerHodingBall()
    {
        if (Balls.Count > 0)
        {
            isPlayerHoldingBall = true;
        }
        else
        {
            isPlayerHoldingBall = false;
        }
    }

    private void Update()
    {
        if (useJoystick)
        {
            Movement();
        }
        else
        {
            joystickClick = false;

            if (isPlayerHoldingBall)
            {
                animator.SetBool("BaLLIdle", true);
                animator.SetBool("BallRunning", false);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idle", true);
                animator.SetBool("BaLLIdle", true);
                animator.SetBool("BallRunning", false);
            }
        }

        CheckIfPlayerHodingBall();

        #region Ball Throw
        if (!joystickClick)
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && countTouch == true && Balls.Count >= 1) || (Input.GetMouseButtonDown(0) && countTouch == true && Balls.Count >= 1))
            {
                ballRigidBody = holdingBall.GetComponent<Rigidbody>();

                StartCoroutine(TouchTimer());
            }
        }

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
            setAngle = false;
        }
    }


    IEnumerator TouchTimer()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("BaLLIdle", false);


        animator.Play("Character_Ball_Throw_Start");

        setAngle = true;
        countTouch = false;
        int k = 0;

        projection.gameObject.SetActive(true);

        while (k <= projectionDelay)
        {
            touchTime += 0.0001f;
            throwSpeed += touchTime * throwFactor;
            Debug.Log(throwSpeed);
            k++;
            yield return new WaitForSeconds(0.1f);

            if(throwSpeed >= maxThrowSpeed)
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
                //Debug.Log(throwSpeed);
                k++;
            }

            else
            {
                break;
            }

            if (throwSpeed >= maxThrowSpeed)
            {
                setAngle = false;
            }

            yield return new WaitForSeconds(0.1f);

        }

    abc:

        projection.gameObject.SetActive(false);
        animator.speed = 1;
        touchTime = 0;

        StartCoroutine(ThrowBall());
    }


    
    #endregion

    void AddTargets()
    {
        BallHandler[] balls = (BallHandler[])GameObject.FindObjectsOfType(typeof(BallHandler));
        foreach (BallHandler ball in balls)
        {
            if ((int)ball.ballColor == (int)PlayerColor && callFunction)
            {
                targetBalls.Add(ball.gameObject);
                targetBalls[j].tag = "Player_Ball";
                j++;
            }
        }
        callFunction = false;
    }


    public string GetPlayerName()
    {
        return playerName.text;
    }


    //Player Movement
    private void Movement()
    {
        rb.velocity = new Vector3(fixedJoystick.Horizontal * movementSpeed, rb.velocity.y, fixedJoystick.Vertical * movementSpeed);
        if ( fixedJoystick.Horizontal != 0f || fixedJoystick.Vertical != 0f)
        {
            joystickClick = true;
            if (isPlayerHoldingBall)
            {
                animator.SetBool("BallRunning", true);
                animator.SetBool("BaLLIdle", false);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Running", true);
                animator.SetBool("Idle", false);
                animator.SetBool("BaLLIdle", false);
                animator.SetBool("BallRunning", false);
            }

            transform.rotation = Quaternion.LookRotation(rb.velocity);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rb.velocity), 5f * Time.deltaTime);
        }
        else
        {
              joystickClick = false;

            if (isPlayerHoldingBall)
            {
                animator.SetBool("BaLLIdle", true);
                animator.SetBool("BallRunning", false);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idle", true);
                animator.SetBool("BaLLIdle", true);
                animator.SetBool("BallRunning", false);
            }
        }
        if (fixedJoystick.Vertical > 0)
        {
            forward = true;
        }
        else if (fixedJoystick.Vertical < 0)
        {
            forward = false;
        }
    }

    //Placing Balls At BallPlace Area When Player is on launchPad
    void PlaceBallsInSelectionArea()
    {
        if (!isBallsPlaced && Balls.Count >= 1)
        {
            for (int i = 1; i < Balls.Count; i++)
            {
                Balls[0].gameObject.tag = "Placed Balls";
                Balls[i].gameObject.tag = "Placed Balls";
                Vector3 pos = BallPlaceArea.gameObject.transform.position;
                pos.y = ballPlaceHeight;
                Balls[i].transform.position = pos;
                Balls[i].transform.parent = BallPlaceArea.transform;
                ballPlaceHeight += ballGapAtBallPlaceArea;
                Debug.Log("Balls" + Balls.Count);
            }
        }
    }

    #region COROUTINES
    IEnumerator FallAndGetup()
    {
        useJoystick = false;
        animator.Play("Falling");

        animator.SetBool("Running", true);
        animator.SetBool("BallRunning", false);


        yield return new WaitForSeconds(1f);

        animator.Play("Running");
        useJoystick = true;
    }
   

    IEnumerator StartAddingTargetsToList()
    {
        while (true)
        {
            AddTargets();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ThrowBall()
    {
        yield return new WaitForSeconds(shootTime);

        ballRigidBody.constraints = RigidbodyConstraints.None;

        animator.SetBool("BaLLIdle", false);
        animator.Play("Character_Ball_Throw_End");

        ballRigidBody.velocity = dir * throwSpeed;
        ballRigidBody.useGravity = true;
        holdingBall.transform.SetParent(null);

        holdingBall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        SphereCollider sphereCollider = holdingBall.transform.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = false;
        if (Balls.Count == 1)
        {
            Balls.RemoveAt(0);
        }

        animator.SetBool("Ball Throw", false);

        if (Balls.Count >= 1)
        {
            animator.SetBool("BallThrow_End", false);
            animator.SetBool("BaLLIdle", true);
        }
        else
        {
            animator.SetBool("BallThrow_End", false);
            animator.SetBool("Idle", true);
        }
        throwSpeed = minThrowSpeed;
        StartCoroutine(StartCountTouch());
    }

    IEnumerator StartCountTouch()
    {
        yield return new WaitForSeconds(0.5f);
        countTouch = true;
        delayAnimation = true;
        if (!(Balls.Count <= 0))
        {
            Debug.Log("AFTER REMOVING " + Balls.Count);
            Balls[Balls.Count - 1].gameObject.transform.position = ballPosition.transform.position;
            Balls[Balls.Count - 1].gameObject.transform.parent = ballPosition.transform;
            holdingBall = Balls[Balls.Count - 1];
            Balls.RemoveAt(Balls.Count - 1);
        }
    }

    #endregion


    #region ONCOLLISION'S
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Yellow_Player") || collision.gameObject.CompareTag("Blue_Player"))
        {
            if (holdingBallCount < collision.gameObject.GetComponent<AIPlayerMovement>().ballHoldingCountAI)
            {
                StartCoroutine(FallAndGetup());

                for (int i = 0; i < holdingBallCount; i++)
                {
                    if (Balls.Count > 0)
                    {
                        Balls[Balls.Count - 1].gameObject.transform.SetParent(null);
                        Balls[Balls.Count - 1].gameObject.GetComponent<BallHandler>().UnsetSetRBConstraints();
                        Balls.RemoveAt(Balls.Count - 1);                    
                    }
                }
                ballCurrentHeight = ballPosition.transform.position.y;
                holdingBallCount = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball Throw Area") && Balls.Count >= 1)
        {
            makeIdle = true;

            holdingBallCount = 0;
            //CameraController.Instance.SetCameraForThrow();
            Camera.main.GetComponent<CameraController>().canZoom = true;
            useJoystick = false;
            movementSpeed = 0f;
            rotationSpeed = Vector3.zero;
            transform.DOMove(ballThrowPos.position, 0.5f);
            transform.LookAt(Basket.position);
            PlaceBallsInSelectionArea();
            isReachedToThrowArea = true;
            if (transform.position == ballThrowPos.position)
            {
                useJoystick = true;
            }
            if (isReachedToThrowArea)
            {
                isThrowingArea = true;
                countTouch = true;
            }
        }
        if (holdingBallCount < 5)
        {
            if (other.gameObject.CompareTag("Player_Ball") && !isThrowingArea)
            {
                Balls.Add(other.gameObject);
                holdingBall = Balls[0];
                ballHold = true;
                Vector3 pos = ballPosition.transform.position;//temporary vector to hold the ball current pos
                pos.y = ballCurrentHeight;//
                other.gameObject.transform.position = pos;//set ball gameobjects position
                other.gameObject.transform.SetParent(ballPosition.transform);//set parent of ball gameobject
                ballCurrentHeight += ballGap;//adding height for next ball
                holdingBallCount += 1;
                //targetBalls[i] = null;
                //targetBalls.RemoveAt(i);
                //i += 1; //change next target
                if (holdingBallCount == 5 && !BallSpawningController.isPlayerBallSpawned)
                {
                    BallSpawningController.Instance.SpawnPlayerBalls();
                    BallSpawningController.isPlayerBallSpawned = false;
                }
            }
        }
        else
        {
            return;
        }
    }

  

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ball Throw Area"))
        {
            Camera.main.GetComponent<CameraController>().canZoom = false;
            isThrowingArea = false;
            countTouch = false;
            isBallsPlaced = false;
            ballCurrentHeight = ballPosition.transform.position.y;
            ballPlaceHeight = BallPlaceArea.position.y;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ball Throw Area"))
        {       
            if (forward)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            if (Balls.Count <= 0)
            {
                useJoystick = true;
                movementSpeed = 5f;
                rotationSpeed = new Vector3(0f, 40f, 0f);
                if(makeIdle)
                {
                    makeIdle = false;
                    animator.Play("Idle");
                }
            }
        }
    }

    
    #endregion

    public Vector3 ReturnDir()
    {
        return dir;
    }

    public float ReturnThrowSpeed()
    {
        return throwSpeed;
    }
}