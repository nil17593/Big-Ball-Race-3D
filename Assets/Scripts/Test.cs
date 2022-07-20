using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
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
    #endregion

    public GameObject ballPosition;
    public float ballGap;
    private float ballCurrentHeight;

    private bool ballHold;

    private Stack<GameObject> balls;
    private List<GameObject> Balls;
    public Transform BallPlaceArea;
    private float ballPlaceHeight;
    public float height;
    private bool isBallsPlaced = false;
    private List<GameObject> placedBalls = new List<GameObject>();


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

    private Rigidbody ballRigidBody;

    #endregion
    private void Awake()
    {
        ballCurrentHeight = ballPosition.transform.position.y;
        ballPlaceHeight = BallPlaceArea.transform.position.y;
        ballHold = false;
        balls = new Stack<GameObject>();
        Balls = new List<GameObject>();

        directionCube.transform.eulerAngles = new Vector3(directionCubeInitAngle, 0, 0);
        countTouch = false;
        delayAnimation = true;
        touchTime = 0;
        throwSpeed = minThrowSpeed;
        dir = directionCube.transform.forward;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        //if (isBallsPlaced)
        //{
        //    if (Input.GetKey(KeyCode.Space))
        //    {
        //        Balls[Balls.Count - 1].gameObject.transform.position = ballPosition.transform.position;
        //        Balls.RemoveAt(Balls.Count - 1);
        //    }
        //}

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && countTouch == true && Balls.Count >= 1) || (Input.GetMouseButtonDown(0) && countTouch == true && Balls.Count >= 1))
        {
            holdingBall = Balls[0];
            animator.SetBool("Ball Idle", false);
            animator.SetBool("Ball Throw", true);

            StartCoroutine(TouchTimer());
        }

        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
            setAngle = false;
        }
    }

    IEnumerator InitiateShoot()
    {
        yield return new WaitForSeconds(0.6f);
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


        while (k <= projectionDelay)
        {
            touchTime += 0.005f;
            throwSpeed += touchTime * throwFactor;
            Debug.Log(throwSpeed);
            k++;
            yield return new WaitForSeconds(0.1f);

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

        holdingBall.transform.SetParent(null);
        //ballRigidBody.constraints = RigidbodyConstraints.None;
        ballRigidBody.velocity = dir * throwSpeed;
        ballRigidBody.useGravity = true;


        animator.SetBool("Ball Throw", false);
        animator.SetBool("Idle", true);

        throwSpeed = minThrowSpeed;
        StartCoroutine(StartCountTouch());
    }

    IEnumerator StartCountTouch()
    {
        yield return new WaitForSeconds(0.5f);
        countTouch = true;
        delayAnimation = true;
    }


    //Player Movement
    private void Movement()
    {
        rb.velocity = new Vector3(fixedJoystick.Horizontal * movementSpeed, rb.velocity.y, fixedJoystick.Vertical * movementSpeed);
        if (fixedJoystick.Horizontal != 0f || fixedJoystick.Vertical != 0f)
        {
            if (ballHold)
            {
                animator.SetBool("Ball Running", true);
                animator.SetBool("Ball Idle", false);
            }
            else
            {
                animator.SetBool("Running", true);
                animator.SetBool("Idle", false);
            }

            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else
        {
            if (ballHold)
            {
                animator.SetBool("Ball Idle", true);
                animator.SetBool("Ball Running", false);
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Idle", true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_Ball"))
        {
            isBallsPlaced = false;
            ballHold = true;
            Vector3 pos = ballPosition.gameObject.transform.position;//temporary vector to hold the ball current pos
            pos.y = ballCurrentHeight;//
            other.gameObject.transform.position = pos;//set ball gameobjects position
            other.gameObject.transform.SetParent(ballPosition.transform);//set parent of ball gameobject
            ballCurrentHeight += ballGap;//adding height for next ball
            Balls.Add(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball Throw Area"))
        {
            PlaceBallsInSelectionArea();
            countTouch = true;
        }
    }

    void PlaceBallsInSelectionArea()
    {
        if (!isBallsPlaced)
        {
            for (int i = 1; i < Balls.Count; i++)
            {
                Balls[i].gameObject.transform.position = BallPlaceArea.position;
                Vector3 pos = BallPlaceArea.gameObject.transform.position;
                pos.y = ballPlaceHeight;
                Balls[i].gameObject.transform.position = pos;
                ballPlaceHeight += height;
                Balls[i].gameObject.transform.parent = BallPlaceArea.transform;
                Balls[i].gameObject.tag = "Placed Balls";
                //Balls.RemoveAt(i);
                //balls.Push(Balls[i]);             
                Debug.Log("PLACEDBALLS" + balls.Count);
                Debug.Log("Balls" + Balls.Count);

                ballCurrentHeight -= 0.5f;
            }
        }
        isBallsPlaced = true;
    }

    void PeekBallsAfterThrow()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = Balls.Count; i > 1; i++)
            {
                Balls[i].gameObject.transform.position = ballPosition.transform.position;
            }
        }
    }
}
