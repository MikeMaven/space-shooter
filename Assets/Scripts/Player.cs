using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = 0.0f;
    private bool _shieldsActive;
    private int _lives = 3;
    private bool _invincible;
    private int _score;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive;
    [SerializeField]
    private GameObject _shieldsPrefab;
    [SerializeField]
    private GameObject _leftEngineDamage, _rightEngineDamage;
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserClip, _powerupClip, _laserHitClip;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3 (0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _shieldsPrefab.SetActive(false);
        _uiManager = GameObject.Find("UICanvas").GetComponent<UIManager>();
        if(!_uiManager)
        {
            Debug.Log("UI Manager is null");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null!");
        }
        _rightEngineDamage.SetActive(false);
        _leftEngineDamage.SetActive(false);
        _audioSource  = transform.GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.LogError("Laser sound audio source not assigned");
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (!_spriteRenderer)
        {
            Debug.LogError("Sprite renderer not assigned on player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        RoomWrap();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
    }

    void RoomWrap() 
    {
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -5)
        {
            transform.position = new Vector3(transform.position.x, -5, 0);
        }

        if (transform.position.x >= 10.25f)
        {
            transform.position = new Vector3(-10.25f, transform.position.y, 0);
        }
        else if (transform.position.x <= -10.25f)
        {
            transform.position = new Vector3(10.25f, transform.position.y, 0);
        }
    }

    void Shoot()
    {
        _canFire = Time.time + _fireRate;
        if(!_isTripleShotActive)
        {
            float yPosition = transform.position.y + 1.5f;
            Instantiate(_laserPrefab, new Vector3(transform.position.x, yPosition, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_tripleShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        }
        _audioSource.PlayOneShot(_laserClip, 1.0f);
    }

    public void PowerupClip()
    {
        _audioSource.PlayOneShot(_powerupClip, 1.0f);
    }

    public void LoseALife()
    {
        if (!_shieldsActive && !_invincible)
        {
            _lives--;
            _audioSource.PlayOneShot(_laserHitClip, 1.0f);
            StartCoroutine(DamageCooldown());
            StartCoroutine(Blink());
            if (_lives == 2)
            {
                _leftEngineDamage.SetActive(true);
            }
            if (_lives == 1)
            {
                _rightEngineDamage.SetActive(true);
            }

            _uiManager.UpdateLives(_lives);
        }
        else
        {
            _shieldsActive = false;
            _shieldsPrefab.SetActive(false);
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _uiManager.GameOver();
            Destroy(this.gameObject, 0.2f);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine ()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplier; 
    }

    public void SetShieldsActive()
    {
        _shieldsActive = true;
        _shieldsPrefab.SetActive(true);
    }
    public void AddToScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    IEnumerator DamageCooldown()
    {
        _invincible = true;
        yield return new WaitForSeconds(2.0f);
        _invincible = false;
    }

    IEnumerator Blink()
    {
        while(_invincible)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.15f);
        }
    }
}
