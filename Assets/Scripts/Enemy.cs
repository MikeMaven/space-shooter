using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

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
    }

    void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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
            _speed = 1;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);
        }
        if(other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if(_player)
            {
                _player.AddToScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 1;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.Play();
            Destroy(this.gameObject, 2.4f);
        }
    }
}
