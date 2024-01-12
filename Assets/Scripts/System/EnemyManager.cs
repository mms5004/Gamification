using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> _enemyList;
    [SerializeField] private GameObject _target;

    private void Start()
    {
        _enemyList = new List<Enemy>();
    }

    private void Update()
    {
        for (int i = _enemyList.Count - 1; i >= 0; i--)
            _enemyList[i].MoveToTarget();
    }

    public void AddEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        Enemy enemy = Instantiate(enemyPrefab, position, rotation).GetComponent<Enemy>();
        enemy.Initialize(_target, this);
        _enemyList.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
