using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> _enemyList;
    [SerializeField] private GameObject _target;
    [SerializeField] private TextMeshProUGUI _kills;
    [SerializeField] private TextMeshProUGUI _loose;
    [SerializeField] private int _killsNumber = 0;
    [SerializeField] private int _looseNumber = 10;
    [SerializeField] private PauseMenu _pauseSystem;

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

    public void RemoveEnemy(Enemy enemy, bool Count)
    {
        if (Count)
        {
            _killsNumber++;
            _kills.text = _killsNumber.ToString();
        }

        else
        {
            _looseNumber--;
            _loose.text = _looseNumber.ToString();

            if (_looseNumber <= 0)
            {
                _pauseSystem.Loose();
            }
        }

        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
