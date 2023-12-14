using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    
    public float CamSpeed = 10;
    public float CamRotationSpeed = 2;
    public GameObject CameraPivot;
    public GameObject Camera;
    [Header("Camera Distance")]
    public float CameraDistance = 7.5f;
    public float CameraDistanceMin = 1.0f;
    public float CameraDistanceMax = 30f;
    public float ScrollSpeed = 3.0f;
    [Header("Camera rotation clamp")]
    public float CameraClampYUp = 20;
    public float CameraClampYDown = -15;
    [Header("Projectile")]
    public GameObject SpawnProjectileLocation;
    public GameObject Projectile;

    void Start()
    {
        Cursor.visible = false;
        Camera.transform.localPosition =  Camera.transform.localPosition.normalized * CameraDistance;
    }

    void Update()
    {
        //camera distance
        float Scroll = Input.GetAxis("Mouse ScrollWheel");
        CameraDistance -= Scroll * ScrollSpeed;
        CameraDistance = Mathf.Clamp(CameraDistance, CameraDistanceMin, CameraDistanceMax);
        Camera.transform.localPosition =  Camera.transform.localPosition.normalized * CameraDistance;

        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(Projectile, SpawnProjectileLocation.transform.position, SpawnProjectileLocation.transform.rotation);       
        }

        //Dirction
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

        #region Rotation

        transform.rotation = Quaternion.Euler(0, Input.GetAxis("Mouse X") * CamRotationSpeed + transform.rotation.eulerAngles.y, 0);
        float RotX = -Input.GetAxis("Mouse Y") * CamRotationSpeed + CameraPivot.transform.rotation.eulerAngles.x;
        Debug.Log(RotX.ToString());


        if (RotX > CameraClampYUp && RotX < 180)
        {
            RotX = CameraClampYUp;
        }
        if(RotX < 360 + CameraClampYDown && RotX > 180)
        {
            RotX = CameraClampYDown;
        }

        CameraPivot.transform.localRotation = Quaternion.Euler(RotX, 0, 0);
        Vector3 LocalRotation = CameraPivot.transform.localRotation.eulerAngles;
        #endregion 
    }
}