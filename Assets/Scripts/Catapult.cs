using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
            _currentProjectile.Throw(_throwingSpeed, _throwingTime);
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
            _state = CatapultState.Throwing;
    }
}
