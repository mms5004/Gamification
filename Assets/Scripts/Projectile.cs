using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    LineRenderer lineRenderer;
    public float angle;
    public float initialVelocity;
    float initialXVelocity;
    float initialYVelocity;
    public float gravity;
    float elapsedTime;



   
    // Start is called before the first frame update
    void Start()
    {
        initialVelocity = 10;
        angle = 45;
        initialXVelocity = initialVelocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        initialYVelocity = initialVelocity * Mathf.Sin(angle * Mathf.Deg2Rad);
        gravity = -9.81f;
        elapsedTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.Translate((initialXVelocity * elapsedTime)* Time.deltaTime , (initialYVelocity +  (0.5f * gravity * Mathf.Pow(elapsedTime,2))) * Time.deltaTime, 0);



    }
   
}
