using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public enum CatapultState
{
    Armed, Throwing, Reloading, Unarmed
}

public class Catapult : MonoBehaviour
{
    [SerializeField] private GameObject _spawnProjectileLocation;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _arm;
    [SerializeField] private GameObject _reloadMechanism;

    [SerializeField] private float _armLength = 6;
    [SerializeField] private float _startAngle = 6;
    [SerializeField] private float _endAngle = 77;

    [SerializeField] private float _throwingTime = 0.1f;
    [SerializeField] private float _reloadingTime = 2;

    private CatapultState _state = CatapultState.Unarmed;
    private Projectile _currentProjectile = null;
    private float _currentAngle = 0;

    private float _throwingAngle = 0;    
    private float _throwingSpeed = 0;
    private float _throwingRotationSpeed = 0;
    private float _reloadRotationSpeed = 0;

    [Header("Visualization")]
    public LineRenderer _lineRenderer;
    [Tooltip("Render the projectile until that timing, sample independent")]
    public float _timeRenderer;
    public int _samples;

    public Gradient _gradientColor;
    public float _fadeInSpeed; //in second
    public float _fadeOutSpeed;//in second

    private Coroutine _visualizationCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        _throwingAngle = _endAngle - _startAngle;

        float throwingArcLength = Mathf.PI * _armLength * _throwingAngle / 180;

        _throwingSpeed = throwingArcLength / _throwingTime;
        _throwingRotationSpeed = _throwingAngle / _throwingTime;
        _reloadRotationSpeed = -_throwingAngle / _reloadingTime;
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
            _currentProjectile.transform.SetPositionAndRotation(_spawnProjectileLocation.transform.position, _spawnProjectileLocation.transform.rotation);
    }

    private void RotateArm(float rotationSpeed)
    {
        _arm.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        _currentAngle = _arm.transform.eulerAngles.x;
    }

    private void Throw()
    {
        RotateArm(_throwingRotationSpeed);

        if (_currentAngle >= _endAngle)
        {
            StopCoroutine(_visualizationCoroutine);           
            StartCoroutine(DisableVisualisation());

            _currentProjectile.Throw(_throwingSpeed);
            _state = CatapultState.Reloading;
        }
    }

    private void Reload()
    {
        RotateArm(_reloadRotationSpeed);
        _reloadMechanism.transform.Rotate(Vector3.up, 5 * _reloadRotationSpeed * Time.deltaTime);

        if (_currentAngle <= _startAngle)
            _state = CatapultState.Unarmed;
    }

    private void Rearmed()
    {
        _currentProjectile = Instantiate(_projectile, _spawnProjectileLocation.transform.position, _spawnProjectileLocation.transform.rotation).GetComponent<Projectile>();
        _state = CatapultState.Armed;
    }

    public void TryThrow()
    {       
        if (_state == CatapultState.Armed)
        {
            _state = CatapultState.Throwing;
            StartCoroutine(EnableVisualisation());
            _visualizationCoroutine = StartCoroutine(ThrowVizualisation());
        }
    }

    //Trail visualization 
    private IEnumerator ThrowVizualisation()
    {
        while (true) //My favorite loop //_state != CatapultState.Reloading
        {
            float elapsedTime = _timeRenderer/_samples;
            float localTime = 0;
            _lineRenderer.positionCount = _samples;
            

            //Set coordinates in world position
            Vector3 initPos = _spawnProjectileLocation.transform.position;

            //Last position before impact
            Vector3 lastPosition = _spawnProjectileLocation.transform.position;

            Vector3 Speed = _spawnProjectileLocation.transform.up * _throwingSpeed;

            bool Impact = false;

            //set individually each point of the array
            for (int i = 0; i < _samples; i++)
            {
                if (Impact)
                {
                    _lineRenderer.SetPosition(i, lastPosition);
                }
                else
                {
                    //Based on Maxime's formula 
                    // Gravity is time independent multiply by time step ?
                    //*weird
                    float gravity = (0.5f * -9.81f * Mathf.Pow(localTime, 2.0f)) * elapsedTime;
                    lastPosition += Speed * elapsedTime + (gravity * Vector3.up);

                    //float gravity = (0.5f * -9.81f * Mathf.Pow(localTime, 2));
                    //Vector3 Pos = initPos + Speed * localTime + (gravity * Vector3.up);

                    _lineRenderer.SetPosition(i, lastPosition);
                    localTime += elapsedTime;
                    if (lastPosition.y < 0.0f) {Impact = true; }
                }
                

            }
            yield return null;
        }
    }

    //Fade In
    private IEnumerator EnableVisualisation()
    {
        float time = 0;
        while (true) // yes I love while(true)
        {
            time += Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, time / _fadeInSpeed);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor*0.25f;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor;

            if(time > _fadeInSpeed) { break; }
            yield return null;
        }
    }
    //Fade Out
    private IEnumerator DisableVisualisation()
    {
        float time = _fadeOutSpeed;        

        while (true) // anyways, why are you reading ?
        {
            time -= Time.deltaTime;
            Vector4 alphaColor = new Vector4(1, 1, 1, time / _fadeOutSpeed);
            _lineRenderer.startColor = _gradientColor.colorKeys[0].color * alphaColor;
            _lineRenderer.endColor = _gradientColor.colorKeys[1].color * alphaColor;

            if(time < 0) { break; }
            yield return null;
        }
    }

    /*private void OnDrawGizmos()
    {
        Vector3 ok = _spawnProjectileLocation.transform.position;
        Gizmos.DrawLine(ok, ok + _spawnProjectileLocation.transform.up * 30);
    }*/
}
