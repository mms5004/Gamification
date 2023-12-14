using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float CamSpeed = 10;
    public float CamRotationSpeed = 2;
    public GameObject CameraPivot;
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * CamSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * CamSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * CamSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * CamSpeed;
        }

        transform.rotation = Quaternion.Euler(0, Input.GetAxis("Mouse X") * CamRotationSpeed + transform.rotation.eulerAngles.y, 0);
        float RotX = -Input.GetAxis("Mouse Y") * CamRotationSpeed + transform.rotation.eulerAngles.x;
        if(RotX < -50)
        {
            RotX = -50;
        }


        CameraPivot.transform.localRotation = Quaternion.Euler(RotX + CameraPivot.transform.rotation.eulerAngles.x, 0, 0);
        Vector3 LocalRotation = CameraPivot.transform.localRotation.eulerAngles;
    }
}