using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 4.0f;
    public Player _player;
    public Animator _anim;
    public AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    public enum EnemyType
    {
        Easy,
        Medium,
        Hard
    }
    public EnemyType enemyType;

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        RecycleMissed();
        if(Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    public virtual void Movement()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void RecycleMissed()
    {
        if (transform.position.y <= -6.41f) {
            transform.position = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player") 
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
        }
        if(other.tag == "Laser" && !other.gameObject.GetComponent<Laser>()._isEnemyLaser)
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
        }
    }
}
