using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private bool _isInPosition;
    [SerializeField]
    private GameObject[] _laserSpawnPoints;
    [SerializeField]
    private GameObject[] _movementWaypoints;
    private GameObject _currentTarget;
    private float _speed = 1.5f;
    private float _canFire = -1;
    private float _fireRate = 1.5f;
    [SerializeField]
    private float _life = 20;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    void Start()
    {
        _currentTarget = _movementWaypoints[0];
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInPosition)
        {
            Attack();
            MovementAI();
        }
        else
        {
            GetInPosition();
        }
    }

    private void GetInPosition()
    {
        if(transform.position.y >= 2.5f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if (transform.position.y <= 2.5f)
        {
            Debug.Log("Boss in position");
            _isInPosition = true;
        }
    }

    private void MovementAI()
    {
        if (Vector3.Distance(_currentTarget.transform.position, transform.position) > 0.5)
        {
            transform.position = Vector3.MoveTowards(transform.position, _movementWaypoints[System.Array.IndexOf(_movementWaypoints, _currentTarget)].transform.position, _speed * Time.deltaTime);
        }
        else if (System.Array.IndexOf(_movementWaypoints, _currentTarget) + 1 < _movementWaypoints.Length)
        {
            _currentTarget = _movementWaypoints[System.Array.IndexOf(_movementWaypoints, _currentTarget) + 1];
        }
        else
        {
            _currentTarget = _movementWaypoints[0];
        }
    }

    private void Attack()
    {
        if (Time.time > _canFire && _isInPosition)
        {
            _canFire = Time.time + _fireRate;
            for (int i = 0; i < _laserSpawnPoints.Length; i++)
            {
                GameObject laser = Instantiate(_laserPrefab, _laserSpawnPoints[i].transform.position, Quaternion.identity);
                laser.GetComponent<Laser>().AssignEnemyLaser();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" && _isInPosition && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
        {
            _life--;
            Destroy(other.gameObject);
        }
        else if (other.tag == "Laser" && _life <= 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Instantiate(_explosionPrefab, transform.Find("WingRight").position, Quaternion.identity);
            Instantiate(_explosionPrefab, transform.Find("WingLeft").position, Quaternion.identity);
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(this.gameObject, 0.4f);
            OnBossDeath();
        }
    }

    private void OnBossDeath()
    {
        _spawnManager.OnPlayerDeathOrWin();
        _uiManager.GameOver();
    }
}
