using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnPointList;
    [SerializeField] private List<GameObject> _enemyClassList;
    [DoNotSerialize] public float SpawnDelay;

    private EnemyManager _enemyManager;
    private Coroutine _spawnCoroutine;


    public void Initialize(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    public void ToggleSpawnLoop(bool activate)
    {
        if(activate)
            _spawnCoroutine = StartCoroutine("SpawnLoop");
        else if (_spawnCoroutine != null)
        {
            foreach (var _spawnPoint in _spawnPointList)
            {
                Door door = _spawnPoint.GetComponent<Door>();
                if (door != null)
                    door.ToggleState(false);
            }
            StopCoroutine(_spawnCoroutine);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            int idSpawn = Random.Range(0, _spawnPointList.Count);
            int idEnemy = Random.Range(0, _enemyClassList.Count);

            Vector3 spawnPosition = _spawnPointList[idSpawn].transform.position;

            foreach (var _spawnPoint in _spawnPointList)
            {
                Door door = _spawnPoint.GetComponent<Door>();
                if (door != null)
                    door.ToggleState(_spawnPoint == _spawnPointList[idSpawn]);
            }

            _enemyManager.AddEnemy(_enemyClassList[idEnemy], spawnPosition, transform.rotation);

            yield return new WaitForSeconds(SpawnDelay);
        }
    }
}