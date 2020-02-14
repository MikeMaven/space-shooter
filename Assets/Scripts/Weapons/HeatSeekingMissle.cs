using UnityEngine;
using System.Collections;
using System.Linq;

public class HeatSeekingMissle : Laser
{
    private GameObject _playerRef;
    private Vector3 _direction;
    [SerializeField]
    private float _relativeSeekSpeed = 0.3f;
    private GameObject _enemyContainer;
    private Transform _closestEnemy;

    void Start()
    {
        _enemyContainer = GameObject.Find("EnemyContainer");
        _playerRef = GameObject.Find("Player");
        Transform[] _onScreenEnemies = _enemyContainer.GetComponentsInChildren<Transform>();
        _onScreenEnemies = _onScreenEnemies.OrderBy(
                enemy => Vector2.Distance(this.transform.position, enemy.transform.position)
            ).ToArray();
        if(_onScreenEnemies.Length > 0)
        {
            _closestEnemy = _onScreenEnemies[0];
        }
    }

    public override void MoveUp()
    {
        float x;
        if (_closestEnemy && _closestEnemy.transform.position.x < transform.position.x)
        {
            x = -_relativeSeekSpeed;
        }
        else if (_closestEnemy && _closestEnemy.transform.position.x > transform.position.x)
        {
            x = _relativeSeekSpeed;
        }
        else
        {
            x = 0;
        }
        _direction = new Vector3(x, 1, 0);
        transform.Translate(_direction * speed * Time.deltaTime);

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public override void MoveDown()
    {
        float x;
        if (_playerRef.transform.position.x < transform.position.x)
        {
            x = -_relativeSeekSpeed;
        }
        else
        {
            x = _relativeSeekSpeed;
        }
        _direction = new Vector3(x, -1, 0);
        transform.Translate(_direction * speed * Time.deltaTime);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
