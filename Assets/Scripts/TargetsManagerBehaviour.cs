using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetsManagerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _targets;
    [SerializeField] private GameObject _catapult;

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private bool _activateFunction;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if(_activateFunction)
            SpawnTarget();
    }

    private void SpawnTarget()
    {
        int targetID = Random.Range(0, _targets.Length);
        float distance = Random.Range(_minDistance, _maxDistance);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        Vector3 relativePosition = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * distance;

        Instantiate(_targets[targetID], relativePosition + _catapult.transform.position, _catapult.transform.rotation);
    }
}
