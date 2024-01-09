using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public enum ProjectileClass
{
    Rock, Bounce, Explode, Freeze, AI
}


public class Projectile : MonoBehaviour
{
    public float Gravity = -9.81f;
    public float WindFactor = 0.01f;
    public bool IsFlying { get; private set; } = false;
    private Vector3 _direction;
    private Vector3 _windVelocity;
    private Vector3 _windDirection;
    private float _speed = 0;
    private float _elapsedTime = 0;

    [Header("Power")]
    public ProjectileClass _classPower;
    public GameObject _explosion;
    public GameObject _freeze;
    public GameObject _IA;

    [Header("Bounce")]
    public float _yVelocityThreshold = 10.0f;
    public float _bounceEnergyTransfer = 0.1f;

    public TrailRenderer _trailRenderer;


    // Start is called before the first frame update
    private void Start()
    {
        _windDirection = Wind.WindForward;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsFlying)
        {
            float delta = Time.deltaTime;
            _elapsedTime += delta;

            transform.Translate(
                _direction.x * _speed * delta,
                (_direction.y * _speed + (0.5f * Gravity * Mathf.Pow(_elapsedTime, 2.0f))) * delta,
                _direction.z * _speed * delta,
                Space.World);

            //Add wind
            //_windVelocity = _windDirection * _elapsedTime * WindFactor;  //Same but time effect apply if bounce
            _windVelocity += _windDirection * delta * WindFactor;
            transform.Translate(_windVelocity * delta, Space.World);



            //Bounce, explode or stop flying ?
            if (transform.position.y <= 0)
            {
                //explode ?
                if (_classPower == ProjectileClass.Explode)
                {
                    Instantiate(_explosion, transform.position, transform.rotation);
                    Destroy(gameObject);
                }

                //Bounce ?
                else if (_classPower == ProjectileClass.Bounce)
                {
                    float yVelocity = _speed + (0.5f * Gravity * Mathf.Pow(_elapsedTime, 2.0f));

                    if (Mathf.Abs(yVelocity) >= _yVelocityThreshold) 
                        _elapsedTime = - _elapsedTime * _bounceEnergyTransfer;
                    else
                        IsFlying = false;
                }

                //Heck no, stop flying !
                else
                {
                    IsFlying = false;
                }
            }      
            

            //Angry bird click explosion
            if(_classPower == ProjectileClass.Freeze && Input.GetMouseButtonDown(1))
            {
                Instantiate(_freeze, transform.position + Vector3.up * 15.0f, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    public void Throw(float initialSpeed)
    {
        IsFlying = true;
        _direction = transform.up;
        _speed = initialSpeed;
        _elapsedTime = 0;
        _trailRenderer.emitting = true;
    }
}