using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    Vector3 Location;
    public float YRotation;
    void Start()
    {
        Location = transform.position;
    }
    void Update()
    {        
        transform.localRotation = Quaternion.Euler(0, Controller.XSlide * 45 , 0);
    }
}