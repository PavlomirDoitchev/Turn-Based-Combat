using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
   

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
            cameraTransform.rotation * Vector3.up);
    }
}

