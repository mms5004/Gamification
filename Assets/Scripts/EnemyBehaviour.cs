using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed;

    private GameObject _target;
    private EnemiesManagerBehaviour _spawnManager;

    public void Initialize(GameObject target, EnemiesManagerBehaviour spawnManager)
    {
        _target = target;
        _spawnManager = spawnManager;
    }

    public void MoveToTarget()
    {
        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0;
        transform.position += _speed * Time.deltaTime * Vector3.Normalize(direction);
        Rect rectangle = new Rect(_target.transform.position.x - 10, _target.transform.position.z - 10, 20, 20);

        if (rectangle.Contains(new Vector2(transform.position.x, transform.position.z)))
        {
            Die();
        }
    }
    public void Die()
    {
        _spawnManager.DespawnTarget(this);
        Destroy(this.gameObject);
    }
    public void Freeze()
    {
        _speed = 0;
    }
}
