using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    static public float Gravity = -9.81f;

    public bool IsThrown { get; private set; } = false;

    private float _angle;
    private float _initialZVelocity;
    private float _initialYVelocity;
    private float _elapsedTime = 0;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (IsThrown)
        {
            _elapsedTime += Time.deltaTime;
            transform.Translate(0, (_initialYVelocity + (0.5f * Gravity * Mathf.Pow(_elapsedTime, 2))) * Time.deltaTime,
                _initialZVelocity * _elapsedTime * Time.deltaTime);
        }
    }

    public void Throw(float initialVelocity)
    {
        IsThrown = true;
        _initialZVelocity = initialVelocity * Mathf.Cos(_angle * Mathf.Deg2Rad);
        _initialYVelocity = initialVelocity * Mathf.Sin(_angle * Mathf.Deg2Rad);
        _angle = 45;
    }
}