using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed;

    private GameObject _target;
    private EnemyManager _enemyManager;

    public void Initialize(GameObject target, EnemyManager enemyManager)
    {
        _target = target;
        _enemyManager = enemyManager;
    }

    public void MoveToTarget()
    {
        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0;
        transform.position += _speed * Time.deltaTime * Vector3.Normalize(direction);
        Rect rectangle = new Rect(_target.transform.position.x - 10, _target.transform.position.z - 10, 20, 20);

        if (rectangle.Contains(new Vector2(transform.position.x, transform.position.z)))
        {
            Die(false);
        }
    }

    public void Die(bool CountAsKill)
    {
        _enemyManager.RemoveEnemy(this, CountAsKill);
    }

    public void Freeze()
    {
        _speed = 0;
    }
}
