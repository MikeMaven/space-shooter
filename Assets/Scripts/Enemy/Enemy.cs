using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected float speed = 4.0f;
    public SpawnManager spawnManager;
    public Player _player;
    public Animator _anim;
    public AudioSource _audioSource;
    public GameObject _laserPrefab;
    [SerializeField]
    private float _fireRateMin = 3.0f;
    [SerializeField]
    private float _fireRateMax = 7.0f;
    private float _fireRate;
    private float _canFire = -1;
    private bool _hasShield;
    public enum EnemyType
    {
        Easy,
        Medium,
        Hard
    }
    public EnemyType enemyType;

    public void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("The player is not assigned in the enemy script");
        }
        
        _anim = GetComponent<Animator>();
        if (!_anim)
        {
            Debug.LogError("The animator component is not present");
        }

        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.LogError("The enemy audio source is not assigned");
        }

        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (!spawnManager)
        {
            Debug.LogError("Spawn Manager note assigned in Enemy script");
        }
    }

    public void Update()
    {
        Movement();
        RecycleMissed();
        if(Time.time > _canFire)
        {
            Shoot();
        }

        if(Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), 2, 1 << 10))
        {
            Shoot();    
        }
    }

    public virtual void Movement()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void Shoot()
    {
        _fireRate = Random.Range(_fireRateMin, _fireRateMax);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    void RecycleMissed()
    {
        if (transform.position.y <= -6.41f) {
            transform.position = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !_hasShield)
        {
            Player player = other.transform.GetComponent<Player>();
            if(player != null) {
                player.LoseALife();
            }
            _anim.SetTrigger("OnEnemyDeath");
            speed = 1;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);
            spawnManager.OnEnemyDeath();
        }
        if (other.tag == "Laser" && !_hasShield && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
        {
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(other.gameObject);
            if(_player)
            {
                _player.AddToScore(10);
            }
            speed = 1;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);
            spawnManager.OnEnemyDeath();
        }
    }

    public void SetShield(bool setting)
    {
        _hasShield = setting;
    }
}
