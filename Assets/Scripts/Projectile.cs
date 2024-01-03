using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    static public float Gravity = -9.81f;

    public bool IsFlying { get; private set; } = false;
    private Vector3 _direction;
    private float _speed = 0;
    private float _elapsedTime = 0;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (IsFlying)
        {
            _elapsedTime += Time.deltaTime;

            transform.Translate(
                _direction.x * _speed * Time.deltaTime,
                (_direction.y * _speed + (0.5f * Gravity * Mathf.Pow(_elapsedTime, 2))) * Time.deltaTime,
                _direction.z * _speed * Time.deltaTime,
                Space.World);

            IsFlying = transform.position.y > 0;
        }
    }

    public void Throw(float initialSpeed, float initialElapsedTime)
    {
        IsFlying = true;
        _direction = transform.up;
        _speed = initialSpeed;
        _elapsedTime = initialElapsedTime;
    }
}