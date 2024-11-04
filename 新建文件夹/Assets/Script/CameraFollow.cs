using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float smoothSpeed = 0.125f; 
    public bool matchTargetRotation = true; 

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        if (matchTargetRotation)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(target.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed);
        }
        else
        {
            transform.LookAt(target);
        }
    }
}
