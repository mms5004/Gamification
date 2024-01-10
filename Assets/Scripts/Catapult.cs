using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public enum CatapultState
{
    Armed, Throwing, Reloading, Unarmed
}

public class Catapult : MonoBehaviour
{
    [Header("Catapult Elements")]
    [SerializeField] private Arm _arm;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _reloadMechanism;
    [SerializeField] private ProjectileClass _currentProjectileClass;

    [Header("Variables")]
    [SerializeField] private float _reloadingTime = 2;
    private float _reloadRotationSpeed = 0;
    [SerializeField] private float _startAngle = 6;
    [SerializeField] private float _endAngle = 77;
    private float _currentArmAngle = 0;
    private float _throwingRangeAngle = 0;
    private float _throwingArcLength = 0;
    private float _throwingSpeed = 0;

    private CatapultState _state = CatapultState.Unarmed;
    private Projectile _currentProjectile = null;

    private Vector3 _spawnProjectilePosition;
    private Quaternion _spawnProjectileRotation;

    [Header("Visualization")]
    public LineRenderer _lineRenderer;
    [Tooltip("Render the projectile until that timing, sample independent")]
    public float _timeRenderer;
    public int _samples;

    public Gradient _gradientColor;
    public float _fadeInDuration;
    public float _fadeOutDuration;

    private Coroutine _visualizationCoroutine;

    public bool WantsToDisplayVisualization = false;


    private void Start()
    {
        UpdateThrowingVariables();

        _arm.ProjectileSpawnLocation.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);
    }

    private void UpdateThrowingVariables()
    {
        _throwingRangeAngle = _endAngle - _startAngle;
        _throwingArcLength = Mathf.PI * _arm.Length * _throwingRangeAngle / 180;
        _throwingSpeed = _throwingArcLength * _arm.RotationSpeed / _throwingRangeAngle;
        _reloadRotationSpeed = -_throwingRangeAngle / _reloadingTime;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_state)
        {
            case CatapultState.Throwing:
                Throw();
                break;
            case CatapultState.Reloading:
                Reload();
                break;
            case CatapultState.Unarmed:
                Rearmed();
                break;
        }
    }

    private void LateUpdate()
    {
        if (_currentProjectile != null && !_currentProjectile.IsFlying)
        {
            _arm.ProjectileSpawnLocation.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);

            _currentProjectile.transform.SetPositionAndRotation(_spawnProjectilePosition, _spawnProjectileRotation);
        }
    }

    private void RotateArm(float rotationSpeed)
    {
        _arm.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        _currentArmAngle = _arm.transform.eulerAngles.x;
    }

    public void TryThrow()
    {
        if (_state == CatapultState.Armed)
        {
            _state = CatapultState.Throwing;
        }
    }

    private void Throw()
    {
        RotateArm(_arm.RotationSpeed);

        if (_currentArmAngle >= _endAngle)
        {
            _currentProjectile.Throw(_throwingSpeed);
            _currentProjectile = null;
            _state = CatapultState.Reloading;
        }
    }

    private void Reload()
    {
        RotateArm(_reloadRotationSpeed);
        _reloadMechanism.transform.Rotate(Vector3.up, 5 * _reloadRotationSpeed * Time.deltaTime);

        if (_currentArmAngle <= _startAngle)
            _state = CatapultState.Unarmed;
    }

    private void Rearmed()
    {
       _currentProjectile = Instantiate(_projectile, _spawnProjectilePosition, _spawnProjectileRotation).GetComponent<Projectile>();
        _currentProjectile._classPower = _currentProjectileClass;
        _state = CatapultState.Armed;

        if(WantsToDisplayVisualization)
            StartVisualization();
    }

    public void StartVisualization()
    {
        _visualizationCoroutine = StartCoroutine(ThrowVisualization());
        StartCoroutine(EnableVisualisation());
    }

    public void StopVisualization()
    {
        StartCoroutine(DisableVisualisation());
    }

    //Trail visualization 
    private IEnumerator ThrowVisualization()
    {
        while (true) //My favorite loop 
        {
            float elapsedTime = _timeRenderer / _samples;

            _lineRenderer.positionCount = _samples;

            Projectile ProjectileScript = _projectile.GetComponent<Projectile>();

            //Project the throwing starting position. Need a double check, not perfectly accurate
            Vector3 pivotPoint = _arm.ProjectileSpawnLocation.transform.position +
                                 _arm.ProjectileSpawnLocation.transform.forward * _arm.Length;
            Vector3 relativeStartPosition = new Vector3(
                0, 
                Mathf.Sin(_endAngle) * _arm.Length, 
                Mathf.Cos(_endAngle) * _arm.Length);
            Vector3 currentPosition = pivotPoint + relativeStartPosition;

            //Vector3 direction = Quaternion.AngleAxis(_endAngle, Vector3.right) * _arm.ProjectileSpawnLocation.transform.up;
            Vector3 direction = Quaternion.AngleAxis(_endAngle, _arm.ProjectileSpawnLocation.transform.right) * _arm.ProjectileSpawnLocation.transform.up;
            Vector3 initialVelocity = direction * _throwingSpeed;

            Vector3 GravitySpeed = Vector3.zero;
            Vector3 WindSpeed = Vector3.zero;

            bool Impact = false;

            //set individually each point of the array
            for (int i = 0; i < _samples; i++)
            {
                if (Impact)
                {
                    _lineRenderer.SetPosition(i, currentPosition);
                }
                else
                {
                    GravitySpeed += 9.807f * elapsedTime * Vector3.down;
                    WindSpeed += ProjectileScript.WindFactor * elapsedTime * Wind.WindForward;

                    Vector3 velocityCurrentOffset = initialVelocity * elapsedTime;
                    Vector3 gravityCurrentOffset = GravitySpeed * elapsedTime;
                    Vector3 windCurrentOffset = WindSpeed * elapsedTime;

                    currentPosition += velocityCurrentOffset + gravityCurrentOffset + windCurrentOffset;


                    //Based on Maxime's formula 
                    // Gravity is time independent multiply by time step ?
                    //*weird
                    
                    
                    //float gravity = (0.5f * -9.81f * Mathf.Pow(localTime, 2.0f)) * elapsedTime;
                    //currentPosition += Speed * elapsedTime + (gravity * Vector3.up);

                    //add wind

                    //WindSpeed = Wind.WindForward * localTime * ProjectileScript.WindFactor;
                    //currentPosition += WindSpeed * elapsedTime;

                    //float gravity = (0.5f * -9.81f * Mathf.Pow(localTime, 2));
                    //Vector3 Pos = initPos + Speed * localTime + (gravity * Vector3.up);

                    _lineRenderer.SetPosition(i, currentPosition);

                    if (currentPosition.y < 0.0f) { Impact = true; }
                }
            }
            yield return null;
        }
    }

    //Fade In
    private IEnumerator EnableVisualisation()
    {
        float elapsedTime = 0;
        while (true) // yes I love while(true)
        {
            elapsedTime += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, elapsedTime / _fadeInDuration);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor * 0.25f;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor;

            if(elapsedTime > _fadeInDuration) { break; }
            yield return null;
        }
    }
    //Fade Out
    private IEnumerator DisableVisualisation()
    {
        float elapsedTime = 0;        

        while (true) // anyways, why are you reading ?
        {
            elapsedTime += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, _fadeOutDuration - elapsedTime / _fadeOutDuration);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor;

            if (elapsedTime > _fadeOutDuration)
            {
                StopCoroutine(_visualizationCoroutine);
                break;
            }
            yield return null;
        }
    }

    /*private void OnDrawGizmos()
    {
        Vector3 ok = _spawnProjectileLocation.transform.position;
        Gizmos.DrawLine(ok, ok + _spawnProjectileLocation.transform.up * 30);
    }*/
}
