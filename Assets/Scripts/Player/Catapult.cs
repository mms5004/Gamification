using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GridBrushBase;
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
    [Header("Elements")]
    [SerializeField] private Arm _arm;
    [SerializeField] private ReloadMechanism _reloadMechanism;
    [SerializeField] private GameObject _armPivot;
    [SerializeField] private GameObject _reloadPivot;
    [SerializeField] private GameObject _armEndingPoint;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _floortarget;
    public ProjectileClass _currentProjectileClass;

    [field: Header("Variables")]
    [field: SerializeField] public float ThrowingAngle { get; private set; } = 76;
    [SerializeField] private float _startAngle = 6;
    [SerializeField] private float _minPivotOffset = 0;
    [SerializeField] private float _maxPivotOffset = 2.8f;

    private float _currentPivotOffset;
    private float _currentArmAngle;
    private float _throwingSpeed; 
    private float _reloadRotationSpeed;

    private CatapultState _state = CatapultState.Unarmed;
    private Projectile _currentProjectile;


    private Vector3 _spawnProjectilePosition;
    private Quaternion _spawnProjectileRotation;

    [Header("Visualization")]
    [SerializeField] private LineRenderer _lineRenderer;
    [Tooltip("Render the projectile until that timing, sample independent"), SerializeField] private float _timeRenderer = 5;
    [SerializeField] private int _samples;

    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.5f;
    [SerializeField] private Gradient _gradientColor;

    private Coroutine _visualizationCoroutine;
    private VisualisationState _visualizationState = VisualisationState.Stopped;
    public bool WantsToVisualize = false;


    private void OnValidate()
    {
        Initialization();
    }


    private void Start()
    {
        Initialization();
    }


    private void Initialization()
    {
        UpdateThrowingVariables(ThrowingAngle);
        _arm.ProjectileSpawnPoint.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);
    }

    public void OnChangeProjectile(ProjectileClass Type)
    {
        if(_currentProjectile) Destroy(_currentProjectile.gameObject);

        _currentProjectileClass = Type;
        _state = CatapultState.Reloading;
    }

    public void OnChangeArm(Arm equippedArm)
    {
        if (_arm)
        {
            if(_currentProjectile) Destroy(_currentProjectile.gameObject);

            Vector3 position = _arm.transform.position;
            Quaternion rotation = _arm.transform.rotation;

            Destroy(_arm.gameObject);
            _state = CatapultState.Reloading;

            _arm = Instantiate(equippedArm, position, rotation);
            _arm.transform.parent = _armPivot.transform;

            UpdateThrowingVariables(ThrowingAngle);
        }
    }


    public void UpdateThrowingVariables(float desiredAngle)
    {
        ThrowingAngle = desiredAngle;

        Vector3 pivotPosition = _armPivot.transform.localPosition;
        Vector3 endingPointPosition = _armEndingPoint.transform.localPosition;
        float endingPointHeight = endingPointPosition.y - pivotPosition.y;
        float endingPointLateralOffset = Mathf.Abs(endingPointPosition.z);

        _currentPivotOffset = endingPointHeight / Mathf.Tan(desiredAngle * Mathf.Deg2Rad) - endingPointLateralOffset;

        if (_currentPivotOffset < _minPivotOffset)
        {
            _currentPivotOffset = _minPivotOffset;
            ThrowingAngle = Mathf.Rad2Deg * Mathf.Atan2(endingPointHeight, _currentPivotOffset + endingPointLateralOffset);

        }

        if (_currentPivotOffset > _maxPivotOffset)
        {
            _currentPivotOffset = _maxPivotOffset;
            ThrowingAngle = Mathf.Rad2Deg * Mathf.Atan2(endingPointHeight, _currentPivotOffset + endingPointLateralOffset);
        }
        _armPivot.transform.localPosition = new Vector3(pivotPosition.x, pivotPosition.y, _currentPivotOffset);

        float throwingRangeAngle = ThrowingAngle - _startAngle;
        float throwingArcLength = _arm.Length * throwingRangeAngle * Mathf.Deg2Rad;

        _throwingSpeed = throwingArcLength * _arm.RotationSpeed / throwingRangeAngle;
        _reloadRotationSpeed = throwingRangeAngle / _reloadMechanism.RotationTime;
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
            _arm.ProjectileSpawnPoint.transform.GetPositionAndRotation(out _spawnProjectilePosition, out _spawnProjectileRotation);

            _currentProjectile.transform.SetPositionAndRotation(_spawnProjectilePosition, _spawnProjectileRotation);
        }
    }

    private void RotateArm(float rotationSpeed)
    {
        _currentArmAngle = Mathf.Clamp(_currentArmAngle + rotationSpeed * Time.deltaTime, _startAngle, ThrowingAngle);
        _arm.transform.localRotation = Quaternion.AngleAxis(_currentArmAngle, Vector3.right);

    }

    public void TryThrow()
    {
        if (_state == CatapultState.Armed)
            _state = CatapultState.Throwing;
    }

    private void Throw()
    {
        RotateArm(_arm.RotationSpeed);

        if (_currentArmAngle >= ThrowingAngle)
        {
            Vector3 direction = Quaternion.AngleAxis(ThrowingAngle, transform.right) * transform.up;

            _currentProjectile.Throw(_throwingSpeed, direction);
            _currentProjectile = null;
            TryStopVisualization();
            _state = CatapultState.Reloading;
        }
    }

    private void Reload()
    {
        RotateArm(-_reloadRotationSpeed);
        _reloadPivot.transform.Rotate(Vector3.up, 5 * _reloadRotationSpeed * Time.deltaTime);

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
        _visualizationState = VisualisationState.FadeOut;
        StopCoroutine(_visualizationCoroutine);
        StartCoroutine(FadeOutVisualization());
    }

    //Trail visualization 
    private IEnumerator ThrowVisualization()
    {
        while (true) //My favorite loop - I let you this one
        {
            float deltaTime = _timeRenderer / _samples;

            _lineRenderer.positionCount = _samples;

            Projectile ProjectileScript = _projectile.GetComponent<Projectile>();

            Quaternion rotation = Quaternion.AngleAxis(180 + (ThrowingAngle - _startAngle), transform.right);
            float distanceBetweenPivotAndSpawnPosition = (_spawnProjectilePosition - _armPivot.transform.position).magnitude;
            Vector3 offset = transform.forward * distanceBetweenPivotAndSpawnPosition;

            Vector3 currentPosition = _armPivot.transform.position + rotation * offset;
            Vector3 direction = Quaternion.AngleAxis(ThrowingAngle, transform.right) * transform.up;
            Vector3 initialVelocity = direction * _throwingSpeed;

            Vector3 gravitySpeed = Vector3.zero;
            Vector3 windSpeed = Vector3.zero;

            bool impact = false;

            //set individually each point of the array
            for (int i = 0; i < _samples; i++)
            {
                if (impact)
                {
                    _lineRenderer.SetPosition(i, currentPosition);
                }
                else
                {
                    gravitySpeed += ProjectileScript.GravityFactor * deltaTime * Vector3.down;
                    windSpeed += ProjectileScript.WindFactor * deltaTime * Wind.WindForward;

                    Vector3 velocityCurrentOffset = initialVelocity * deltaTime;
                    Vector3 gravityCurrentOffset = gravitySpeed * deltaTime;
                    Vector3 windCurrentOffset = windSpeed * deltaTime;

                    currentPosition += velocityCurrentOffset + gravityCurrentOffset + windCurrentOffset;

                    _lineRenderer.SetPosition(i, currentPosition);

                    if (currentPosition.y < 0.0f) 
                    {
                        _floortarget.SetActive(true);
                        _floortarget.transform.position = new Vector3(currentPosition.x,0, currentPosition.z) + Vector3.up * 0.05f;
                        impact = true; 
                    }
                }
            }
            if (!impact)
            {
                _floortarget.SetActive(false);
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
    }

    //Fade Out
    private IEnumerator FadeOutVisualization()
    {
        float elapsedTime = 0;

        while(true) // anyways, why are you reading ?
        {
            elapsedTime += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, 1 - elapsedTime / _fadeOutDuration);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor * 0.5f;

            if (elapsedTime > _fadeOutDuration) break;
            yield return null;
        }
        _floortarget.SetActive(false);
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
