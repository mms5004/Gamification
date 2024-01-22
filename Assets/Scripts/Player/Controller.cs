using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    
    [SerializeField] private float CamSpeed = 10;
    [SerializeField] private float CamRotationSpeed = 2;
    [SerializeField] private GameObject CameraPivot;
    [SerializeField] private GameObject Camera;


    [Header("Camera Distance")]
    [SerializeField] private float _CameraDistance = 7.5f;
    [SerializeField] private float _CameraDistanceMin = 1.0f;
    [SerializeField] private float _CameraDistanceMax = 30f;
    [SerializeField] private float _ScrollSpeed = 3.0f;


    [Header("Camera rotation clamp")]
    [SerializeField] private float _CameraClampYUp = 20;
    [SerializeField] private float _CameraClampYDown = -15;

    private Catapult _Catapult;

    static public float _XSlide;
    static public Rigidbody _RigidBody;

    void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Camera.transform.localPosition =  Camera.transform.localPosition.normalized * _CameraDistance;
        _RigidBody = GetComponent<Rigidbody>();
        _Catapult = this.GetComponent<Catapult>();
    }

    void Update()
    {
        if (PauseMenu.isPaused || PauseMenu.isFinised)
        {
            return;
        }
        //camera distance
        float Scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Scroll != 0)
        {
            /*_CameraDistance -= Scroll * _ScrollSpeed;
            _CameraDistance = Mathf.Clamp(_CameraDistance, _CameraDistanceMin, _CameraDistanceMax);

            Camera.transform.localPosition = Camera.transform.localPosition.normalized * _CameraDistance;*/

            _Catapult.UpdateThrowingVariables(_Catapult.ThrowingAngle + Scroll * 20);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _Catapult.WantsToVisualize = true;
            _Catapult.TryStartVisualization();
        }
        if(Input.GetMouseButtonUp(0))
        {
            _Catapult.WantsToVisualize = false;
            _Catapult.TryThrow();
        }

        Direction();

        #region Rotation

        transform.rotation = Quaternion.Euler(0, Input.GetAxis("Mouse X") * CamRotationSpeed + transform.rotation.eulerAngles.y, 0);
        float RotX = -Input.GetAxis("Mouse Y") * CamRotationSpeed + CameraPivot.transform.rotation.eulerAngles.x;
        _XSlide += Input.GetAxis("Mouse X") * 0.01f;
        _XSlide = Mathf.Clamp(_XSlide,-1.0f,1.0f);

        if (RotX > _CameraClampYUp && RotX < 180)
        {
            RotX = _CameraClampYUp;
        }
        if(RotX < 360 + _CameraClampYDown && RotX > 180)
        {
            RotX = _CameraClampYDown;
        }

        CameraPivot.transform.localRotation = Quaternion.Euler(RotX, 0, 0);
        Vector3 LocalRotation = CameraPivot.transform.localRotation.eulerAngles;
        #endregion 
    }

    private void Direction()
    {
        Vector3 Dir = transform.right * _XSlide + transform.forward;
        if (Input.GetKey(KeyCode.W))
        {
            _RigidBody.velocity = Dir * CamSpeed;
            WheelForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            _RigidBody.velocity = -Dir * CamSpeed;
            WheelForward();
        }
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            _RigidBody.velocity = Vector3.zero;
            WheelForward();
        }
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.position += transform.right * Time.deltaTime * CamSpeed;
        //    XSlide += Time.deltaTime;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.position -= transform.right * Time.deltaTime * CamSpeed;
        //    XSlide -= Time.deltaTime;
        //}
    }

    void WheelForward()
    {
        if (_XSlide > 0)
        {
            _XSlide -= Time.deltaTime;
        }
        else if(_XSlide < 0)
        {
            _XSlide += Time.deltaTime;
        }
    }


}