using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player attributes
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = 0.0f;
    private bool _shieldsActive;
    [SerializeField]
    private int _shieldLife;
    [SerializeField]
    private int _ammo = 15;
    private bool _invincible;
    private int _score;
    private bool _isTripleShotActive;
    private bool _isSpreadShotActive;
    private float _thrusters;


    // Component references
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _spreadShotPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager; 
    [SerializeField]
    private GameObject _shieldsPrefab;
    [SerializeField]
    private GameObject _leftEngineDamage, _rightEngineDamage;
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserClip, _powerupClip, _laserHitClip;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _shieldsRenderer;
    private CameraShake _cameraShake;
    

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

        _shieldsRenderer = _shieldsPrefab.GetComponent<SpriteRenderer>();
        if (!_shieldsRenderer)
        {
            Debug.LogError("Sprite renderer not assigned for shields on player");
        }

        _cameraShake = GameObject.Find("MainCamera").transform.GetComponent<CameraShake>();
        if (!_cameraShake)
        {
            Debug.LogError("Camera shake script not assigned");
        }

        _cameraShake.enabled = false;
        _rightEngineDamage.SetActive(false);
        _leftEngineDamage.SetActive(false);
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
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = _speed * 1.3f;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _speed / 1.3f;
        }
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

        if (_ammo > 0)
        {
            _ammo--;
            _uiManager.UpdateAmmo(_ammo);
            _audioSource.PlayOneShot(_laserClip, 1.0f);
        }

        if(!_isTripleShotActive && _ammo > 0)
        {
            if (_isSpreadShotActive)
            {
                Instantiate(_spreadShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                float yPosition = transform.position.y + 1.5f;
                Instantiate(_laserPrefab, new Vector3(transform.position.x, yPosition, 0), Quaternion.identity);
            }
        }
        else if (_isTripleShotActive && _ammo > 0)
        {
            Instantiate(_tripleShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        }
    }

    public void PowerupClip()
    {
        _audioSource.PlayOneShot(_powerupClip, 1.0f);
    }

    public void LoseALife()
    {
        if (!_shieldsActive && _shieldLife <= 0 && !_invincible)
        {
            _lives--;
            _cameraShake.enabled = true;
            _cameraShake.ResetShakeDuration(1.0f);
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
        else if (_shieldsActive && !_invincible)
        {
            _shieldLife--;
            StartCoroutine(DamageCooldown());
            if (_shieldLife == 2)
            {
                Color temp = _shieldsRenderer.color;
                temp.a = 0.66f;
                _shieldsRenderer.color = temp; 
            }
            else if (_shieldLife == 1)
            {
                Color temp = _shieldsRenderer.color;
                temp.a = 0.33f;
                _shieldsRenderer.color = temp; 
            }
            else if (_shieldLife <= 0)
            {
                Color temp = _shieldsRenderer.color;
                temp.a = 1f;
                _shieldsRenderer.color = temp; 
                _shieldsActive = false;
                _shieldsPrefab.SetActive(false);
            }
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

    public void SpreadShotActive()
    {
        _isSpreadShotActive = true;
        StartCoroutine(SpreadShotPowerDownRoutine());
    }

    IEnumerator SpreadShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpreadShotActive = false;
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
        Color temp = _shieldsRenderer.color;
        temp.a = 1f;
        _shieldsRenderer.color = temp;
        _shieldsActive = true;
        _shieldLife = 3;
        _shieldsPrefab.SetActive(true);
    }
    public void AddToScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void RefillAmmo(int ammo)
    {
        _ammo += ammo;
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
