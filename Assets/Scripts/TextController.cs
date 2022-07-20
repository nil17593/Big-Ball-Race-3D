using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public Transform target;

    public Vector3 offSet;

    private void Update()
    {
        Vector3 targetPos = target.position + offSet;
        transform.position = targetPos;
    }
}
