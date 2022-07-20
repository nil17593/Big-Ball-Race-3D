using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;//target for camera
    [SerializeField]
    private Vector3 offset;//distance between camera and player
    [SerializeField]
    private float smoothFactor;//soothfactor for camera
    [SerializeField]
    private Vector3 offsetZoom;//distance between camera and player
    public bool canZoom;

    public static CameraController Instance;

    private void Start()
    {
        Instance = this;
    }

    //follow player
    void FollowPlayer()
    {
        if (target != null)
        {
            Vector3 targetPos;
            if(!canZoom)
            {
                targetPos = target.position + offset;
                this.transform.rotation = Quaternion.Euler(new Vector3(30,0,0));

            }
            else
            {
                targetPos = target.position + offsetZoom;
                this.transform.rotation = Quaternion.Euler(new Vector3(12, 0, 0));
            }
              
            Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);
            transform.position = targetPos;
        }
    }
    
    private void LateUpdate()
    {
        FollowPlayer();
    }

    public void SetCameraForThrow()
    {
        transform.position = new Vector3(-24f, 11f, 15.14f);
        //transform.rotation = new Vector3(15.4f, 86f, 0f);
    }
}
