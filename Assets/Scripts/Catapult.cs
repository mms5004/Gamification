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
    [SerializeField] private GameObject _SpawnProjectileLocation;
    [SerializeField] private GameObject _Projectile;
    [SerializeField] private GameObject _Arm;

    [SerializeField] private float _minArmAngle = 6;
    [SerializeField] private float _maxArmAngle = 77;

    [SerializeField] private float _throwingRotationSpeed = 250;
    [SerializeField] private float _reloadingRotationSpeed = 50;

    private CatapultState _state = CatapultState.Unarmed;
    private Projectile _currentProjectile = null;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (_currentProjectile != null && !_currentProjectile.IsThrown)
            _currentProjectile.transform.SetPositionAndRotation(_SpawnProjectileLocation.transform.position, _SpawnProjectileLocation.transform.rotation);

        switch (_state)
        {
            case CatapultState.Throwing:
                RotateArm(_throwingRotationSpeed);
                break;
            case CatapultState.Reloading:
                RotateArm(-_reloadingRotationSpeed);
                break;
            case CatapultState.Unarmed:
                TryReload();
                break;
        }
    }

    private void RotateArm(float rotationSpeed)
    {
        _Arm.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        float currentAngle = _Arm.transform.eulerAngles.x;

        if (currentAngle <= _minArmAngle)
            _state = CatapultState.Unarmed;

        else if (currentAngle >= _maxArmAngle)
        {
            _currentProjectile.Throw(_throwingRotationSpeed);
            _state = CatapultState.Reloading;
        }
    }

    public void TryThrow()
    {
        if (_state == CatapultState.Armed)
            _state = CatapultState.Throwing;
    }

    public void TryReload()
    {
        if (_state == CatapultState.Unarmed)
        {
            _currentProjectile = Instantiate(_Projectile, _SpawnProjectileLocation.transform.position, _SpawnProjectileLocation.transform.rotation).GetComponent<Projectile>();
            _state = CatapultState.Armed;
        }
    }
}
