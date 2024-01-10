using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

public enum VisualisationState
{
    FadeIn, Running, FadeOut, Stopped
}

public class Catapult : MonoBehaviour
{
    [Header("Catapult Elements")]
    [SerializeField] private Arm _arm;
    [SerializeField] private GameObject _armPivot;
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
    [SerializeField] private LineRenderer _lineRenderer;
    [Tooltip("Render the projectile until that timing, sample independent"), SerializeField] private float _timeRenderer;
    [SerializeField] private int _samples;

    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;
    [SerializeField] private Gradient _gradientColor;

    private Coroutine _visualizationCoroutine;
    private VisualisationState _visualizationState = VisualisationState.Stopped;
    public bool WantsToVisualize = false;


    private void Start()
    {
        UpdateThrowingVariables();

        _arm.ProjectileSpawn.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);
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
            _arm.ProjectileSpawn.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);

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
            _state = CatapultState.Throwing;
    }

    private void Throw()
    {
        RotateArm(_arm.RotationSpeed);

        if (_currentArmAngle >= _endAngle)
        {
            
            Debug.Log("Actual pos : " + _arm.ProjectileSpawn.transform.position);
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
        _currentProjectile.ClassPower = _currentProjectileClass;
        _state = CatapultState.Armed;

        if (WantsToVisualize)
            TryStartVisualization();
    }

    public void TryStartVisualization()
    {
        if (_state == CatapultState.Armed && _visualizationState == VisualisationState.Stopped)
        {
            _visualizationState = VisualisationState.FadeIn;
            _visualizationCoroutine = StartCoroutine(ThrowVisualization());
            StartCoroutine(FadeInVisualization());
        }
    }

    public void TryStopVisualization()
    {
        if (_visualizationState == VisualisationState.Running)
        {
            _visualizationState = VisualisationState.FadeOut;
            StopCoroutine(_visualizationCoroutine);
            StartCoroutine(FadeOutVisualization());
        }
    }

    //Trail visualization 
    private IEnumerator ThrowVisualization()
    {
        while (true) //My favorite loop - I let you this one
        {
            float elapsedTime = _timeRenderer / _samples;

            _lineRenderer.positionCount = _samples;

            Projectile ProjectileScript = _projectile.GetComponent<Projectile>();

            float distanceBetweenPivotAndSpawnPosition = (_spawnProjectilePosition - _armPivot.transform.position).magnitude;
            Vector3 projectedRelativeSpawnPosition = new Vector3(
                0,
                Mathf.Sin(_endAngle) * distanceBetweenPivotAndSpawnPosition,
                Mathf.Cos(_endAngle) * distanceBetweenPivotAndSpawnPosition);

            Vector3 currentPosition = _armPivot.transform.position + projectedRelativeSpawnPosition;

            Vector3 direction = Quaternion.AngleAxis(_endAngle, transform.right) * transform.up;
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
                    GravitySpeed += ProjectileScript.GravityFactor * elapsedTime * Vector3.down;
                    WindSpeed += ProjectileScript.WindFactor * elapsedTime * Wind.WindForward;

                    Vector3 velocityCurrentOffset = initialVelocity * elapsedTime;
                    Vector3 gravityCurrentOffset = GravitySpeed * elapsedTime;
                    Vector3 windCurrentOffset = WindSpeed * elapsedTime;

                    currentPosition += velocityCurrentOffset + gravityCurrentOffset + windCurrentOffset;

                    _lineRenderer.SetPosition(i, currentPosition);

                    if (currentPosition.y < 0.0f) { Impact = true; }
                }
            }
            yield return null;
        }
    }

    //Fade In
    private IEnumerator FadeInVisualization()
    {
        float elapsedTime = 0;

        while(true) // yes I love while(true)
        {
            elapsedTime += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, elapsedTime / _fadeInDuration);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor * 0.25f;

            if (elapsedTime > _fadeInDuration) break;
            yield return null;

        }

        _visualizationState = VisualisationState.Running;

        if (!WantsToVisualize)
            TryStopVisualization();
    }

    //Fade Out
    private IEnumerator FadeOutVisualization()
    {
        float elapsedTime = 0;

        while(true) // anyways, why are you reading ?
        {
            elapsedTime += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, _fadeOutDuration - elapsedTime / _fadeOutDuration);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor * 0.25f;

            if (elapsedTime > _fadeOutDuration) break;
            yield return null;
        }

        _visualizationState = VisualisationState.Stopped;

        if (WantsToVisualize)
            TryStartVisualization();
    }

    /*private void OnDrawGizmos()
    {
        Vector3 ok = _spawnProjectileLocation.transform.position;
        Gizmos.DrawLine(ok, ok + _spawnProjectileLocation.transform.up * 30);
    }*/
}
