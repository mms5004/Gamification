using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private Vector3 Location;
    private float YRotation;
    void Start()
    {
        Location = transform.position;
    }
    void Update()
    {        
        transform.localRotation = Quaternion.Euler(0,  Controller._XSlide * 45, 90);
    }
}