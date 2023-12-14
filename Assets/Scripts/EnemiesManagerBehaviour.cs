using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class EnemiesManagerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private List<EnemyBehaviour> _spawnedEnemies;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _flag;

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private float _spawnDelay;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine("SpawnLoop");
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            _spawnedEnemies[i].MoveToTarget();
        }
    }
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnTarget();
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void SpawnTarget()
    {
        int enemyID = Random.Range(0, _enemies.Length);
        float distance = Random.Range(_minDistance, _maxDistance);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        Vector3 relativePosition = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * distance;

        EnemyBehaviour enemy =
            Instantiate(_enemies[enemyID], relativePosition + _player.transform.position, _player.transform.rotation)
                .GetComponent<EnemyBehaviour>();
        enemy.Initialize(_flag, this);

        _spawnedEnemies.Add(enemy);
    }

    public void DespawnTarget(EnemyBehaviour enemy)
    {
        _spawnedEnemies.Remove(enemy);
    }
}
