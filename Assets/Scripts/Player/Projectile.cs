using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public enum ProjectileClass
{
    Rock, Bounce, Explode, Freeze, AI
}


public class Projectile : MonoBehaviour
{
    [Header("Power")]
    public ProjectileClass ClassPower;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _freeze;
    [SerializeField] private GameObject _damageZone;

    [Header("Bounce")]
    [SerializeField] private float _verticalBounceSpeedThreshold = 1;

    public TrailRenderer _trailRenderer;
    [field: SerializeField] public float GravityFactor { get; } = 9.80665f;
    [field: SerializeField] public float WindFactor { get; } = 3f;
    public bool IsFlying { get; private set; }

    private Vector3 _initialVelocity;
    private Vector3 _gravityCurrentSpeed = Vector3.zero;
    private Vector3 _windCurrentSpeed = Vector3.zero;

    private void Update()
    {
        if (IsFlying)
        {
            float deltaTime = Time.deltaTime;
            
            _gravityCurrentSpeed += GravityFactor * deltaTime * Vector3.down;
            _windCurrentSpeed += WindFactor * deltaTime * Wind.WindForward;

            Vector3 velocityCurrentOffset = _initialVelocity * deltaTime;
            Vector3 gravityCurrentOffset = _gravityCurrentSpeed * deltaTime;
            Vector3 windCurrentOffset = _windCurrentSpeed * deltaTime;

            transform.Translate(velocityCurrentOffset + gravityCurrentOffset + windCurrentOffset, Space.World);


            //Bounce, explode or stop flying ?
            if (transform.position.y <= 0)
            {
                //explode ?
                if (ClassPower == ProjectileClass.Explode)
                {
                    Instantiate(_explosion, transform.position, transform.rotation);
                    Destroy(gameObject);
                }

                //Bounce ?
                else if (ClassPower == ProjectileClass.Bounce)
                {
                    _initialVelocity *= 0.7f;
                    _gravityCurrentSpeed = Vector3.zero;

                    if (_initialVelocity.y < _verticalBounceSpeedThreshold)
                        Destroy(gameObject);
                }

                //Heck no, stop flying !
                else
                {
                    Destroy(gameObject);
                }
            }      
            

            //Angry bird click explosion
            if(ClassPower == ProjectileClass.Freeze && Input.GetMouseButtonDown(1))
            {
                Instantiate(_freeze, transform.position + Vector3.up * 15.0f, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    public void Throw(float initialSpeed, Vector3 direction)
    {
        IsFlying = true;
        _trailRenderer.emitting = true;
        _initialVelocity = direction * initialSpeed;


        if (ClassPower != ProjectileClass.Freeze)
        {
            GameObject damageZoneObject = Instantiate(_damageZone, transform.position, transform.rotation);
            damageZoneObject.transform.parent = this.transform;
        }
    }
}