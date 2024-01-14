using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmEnum
{
     Base, Short, Long
}

public class Arm : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject _beam;
    [SerializeField] private GameObject _basket;
    [field: SerializeField] public GameObject ProjectileSpawnPoint { get; private set; }

    [field: Header("Variables")]
    [field: SerializeField, Tooltip("In Deg/s")] public float RotationSpeed { get; private set; } = 1f;
    [field: SerializeField] public float Length { get; private set; } = 6;


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
        float basketLength = _basket.transform.localScale.z;
        float beamLength = Length - basketLength;

        _basket.transform.localPosition = new Vector3(0, 0, -(beamLength + 0.5f * basketLength));

        _beam.transform.localScale = new Vector3(0.5f, 0.5f, beamLength);
        _beam.transform.localPosition = new Vector3(0, 0, -0.5f * beamLength);
    }
}
