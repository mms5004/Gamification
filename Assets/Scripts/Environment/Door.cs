using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public enum DoorState
{
    Open, IsOpening, Close, IsClosing
}

public class Door : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _rightDoor;

    [SerializeField, Tooltip("in deg/s")] private float _openingTime = 0.4f;
    [SerializeField] private float _openingRotation = 90;
    private float _currentRotation;
    private float _openingSpeed;

    private DoorState _doorState = DoorState.Close;

    private void Start()
    {
        _openingSpeed = _openingRotation / _openingTime;
    }

    private void Update()
    {
        if (_doorState == DoorState.IsOpening)
            RotateDoor(_openingSpeed);
        else if (_doorState == DoorState.IsClosing)
            RotateDoor(-_openingSpeed);
    }

    private void RotateDoor(float rotationSpeed)
    {
        _currentRotation = Mathf.Clamp(_currentRotation + rotationSpeed * Time.deltaTime, 0, _openingRotation);

        _leftDoor.transform.localRotation = Quaternion.Euler(0, _currentRotation, 0);
        _rightDoor.transform.localRotation = Quaternion.Euler(0, -_currentRotation, 0);

        if (_currentRotation >= _openingRotation)
            _doorState = DoorState.Open;
        else if (_currentRotation <= 0)
            _doorState = DoorState.Close;
    }

    public void ToggleState(bool open)
    {
        if (open)
            _doorState = DoorState.IsOpening;
        else
            _doorState = DoorState.IsClosing;
    }
}
