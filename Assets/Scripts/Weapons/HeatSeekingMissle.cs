using UnityEngine;
using System.Collections;

public class HeatSeekingMissle : Laser
{
    private GameObject _playerRef;
    private Vector3 _direction;
    [SerializeField]
    private float _relativeSeekSpeed = 0.5f;

    void Start()
    {
        _playerRef = GameObject.Find("/Player");
        Debug.Log(_playerRef);
        isEnemyLaser = true;
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
