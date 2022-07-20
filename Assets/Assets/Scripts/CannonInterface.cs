using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CannonInterface : MonoBehaviour 
{

    public bool isMoving;
    public float Speed;
    public bool Move;
    [SerializeField]
    Cursor targetCursor;

    [SerializeField]
    CannonController cannon;

    [SerializeField]
    Text timeOfFlightText;

    [SerializeField]
    float defaultFireSpeed = 35;

    [SerializeField]
    float defaultFireAngle = 45;

    public float initialFireAngle;
    private float initialFireSpeed;
    private bool useLowAngle;

    public bool useInitialAngle;

    void Awake()
    {
        useLowAngle = true;

        initialFireAngle = defaultFireAngle;
        initialFireSpeed = defaultFireSpeed;

        useInitialAngle = true;
    }

    void Update()
    {
        if(isMoving)
        {
            if(targetCursor.gameObject.transform.localPosition.z<61)
            targetCursor.gameObject.transform.Translate(new Vector3(0, 0, 1) * Speed * Time.deltaTime);
        }
        else
        {
            targetCursor.gameObject.transform.localPosition = new Vector3(0, 6, -5);
        }
        if (useInitialAngle)
            cannon.SetTargetWithAngle(targetCursor.transform.position, initialFireAngle);
        else
            cannon.SetTargetWithSpeed(targetCursor.transform.position, initialFireSpeed, useLowAngle);

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            cannon.Fire();
        }

        timeOfFlightText.text = Mathf.Clamp(cannon.lastShotTimeOfFlight - (Time.time - cannon.lastShotTime), 0, float.MaxValue).ToString("F3");
        

        if(Move)
        {
            targetCursor.transform.Translate(Vector3.forward * 5*Time.deltaTime);
        }
    }

    public void SetInitialFireAngle(string angle)
    {
        initialFireAngle = Convert.ToSingle(angle);
    }

    public void SetInitialFireSpeed(string speed)
    {
        initialFireSpeed = Convert.ToSingle(speed);
    }

    public void SetLowAngle(bool useLowAngle)
    {
        this.useLowAngle = useLowAngle;
    }

    public void UseInitialAngle(bool value)
    {
        useInitialAngle = value;
    }


    public void Touching()
    {
        isMoving = true;
    }

    public void Shoot()
    {
        cannon.Fire();
        isMoving = false;

    }
}
