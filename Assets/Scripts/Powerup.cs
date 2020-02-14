using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerupId;
    private GameObject _player;
    [SerializeField]
    private GameObject _explosionPrefab;

    void Start()
    {
        _player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        
        if (transform.position.y <= -7.0f)
        {
            Destroy(this.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.PowerupClip();
                switch(_powerupId)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.SetShieldsActive();
                        break;
                    case 3:
                        player.RefillAmmo(15);
                        break;
                    case 4:
                        player.NegativePowerupActive();
                        break;
                    case 5:
                        player.SpreadShotActive();
                        break;
                    case 6:
                        player.HomingMissleActive();
                        break;
                    case 7:
                        player.HealPlayer();
                        break;
                    default:
                        Debug.LogError("Something went wrong");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }

    public void Explode()
    {
        Destroy(this.gameObject, 0.2f);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }
}
