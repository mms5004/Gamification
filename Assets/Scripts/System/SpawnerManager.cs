using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[System.Serializable]
public struct EnemyWave
{
    public float Duration;
    public float SpawnDelay;
    public int ActivatedSpawnerNumber;
}

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private List<GameObject> _spawnerClassList;
    [SerializeField] private List<EnemyWave> _enemyWaveList;
    [SerializeField] private PauseMenu _pauseSystem;

    [SerializeField] private float _minSpawnerDistance = 150;
    [SerializeField] private float _maxSpawnerDistance = 200;
    [SerializeField] private float _spawnerNumber = 3;
    [SerializeField] private float _waveDelay = 10;

    private int _currentWaveIndex;
    private float _elapsedTime;
    private bool _bIsWaveStarted;

    private List<Spawner> _spawnerList;

    private void Start()
    {
        _spawnerList = new List<Spawner>();
        for (int i = 0; i < _spawnerNumber; i++)
            SpawnSpawner();

        StartCoroutine("GameLoop");
    }

    private void SpawnSpawner()
    {
        int idSpawner = Random.Range(0, _spawnerClassList.Count);
        float distance = Random.Range(_minSpawnerDistance, _maxSpawnerDistance);
        float angle = Random.Range(0, 360);

        Quaternion spawnerRotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 spawnerPosition = spawnerRotation * Vector3.left * distance;

        Spawner spawner = Instantiate(_spawnerClassList[idSpawner], spawnerPosition, spawnerRotation)
            .GetComponent<Spawner>();

        spawner.Initialize(_enemyManager);
        _spawnerList.Add(spawner);
    }

    IEnumerator GameLoop()
    {

        while (true)
        {
            if (_currentWaveIndex >= _enemyWaveList.Count)
            {
                _pauseSystem.Win();
                yield break;
            }

            if (!_bIsWaveStarted)
            {
                StartWave(_enemyWaveList[_currentWaveIndex]);
                _bIsWaveStarted = true;

                yield return new WaitForSeconds(_enemyWaveList[_currentWaveIndex].Duration);
            }

            else
            {
                StopWave();
                _bIsWaveStarted = false;
                _currentWaveIndex++;

                yield return new WaitForSeconds(_waveDelay);
            }
        }
    }

    private void StartWave(EnemyWave wave)
    {
        List<int> remainingSpawnerId = new List<int>();

        for (int i = 0; i < _spawnerList.Count; i++)
            remainingSpawnerId.Add(i);

        for (int i = 0; i < wave.ActivatedSpawnerNumber; i++)
        {
            int idSpawner = remainingSpawnerId[Random.Range(0, remainingSpawnerId.Count - 1)];
            remainingSpawnerId.Remove(idSpawner);

            _spawnerList[idSpawner].SpawnDelay = wave.SpawnDelay;
            _spawnerList[idSpawner].ToggleSpawnLoop(true);
        }
    }

    private void StopWave()
    {
        List<Spawner> remainingSpawnerList = _spawnerList;
        foreach (var spawner in _spawnerList)
            spawner.ToggleSpawnLoop(false);
    }
}
