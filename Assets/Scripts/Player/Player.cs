using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Player attributes
    [SerializeField]
    private float _speed = 5.0f;
    private float _originalSpeed;
    private float _newSpeed;
    private bool _leftShift;
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
    [SerializeField]
    private int _maxAmmo = 60;
    private bool _invincible;
    private int _score;
    [SerializeField]
    private float _thrusterFuel = 15.0f;

    //Cached yields
    private WaitForSeconds _thrusterYield;
    private WaitForSeconds _blinkYield;
    private WaitForSeconds _powerupYield;

    //Script references
    private PlayerControls _playerControls;
    private CameraShake _cameraShake;

    // Component references
    private GameObject _selectedWeaponPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _spreadShotPrefab;
    [SerializeField]
    private GameObject _homingMisslePrefab;
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
    private Slider _thrusterSlider;


    void Awake()
    {
        transform.position = new Vector3(0, 0, 0);
        _shieldsPrefab.SetActive(false);
    }
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null!");
        }

        _uiManager = GameObject.Find("UICanvas").GetComponent<UIManager>();
        if(!_uiManager)
        {
            Debug.Log("UI Manager is null");
        }

        _thrusterSlider = GameObject.Find("UICanvas").GetComponentInChildren<Slider>();

        if(!_thrusterSlider)
        {
            Debug.LogError("Slider value not captured on player");
        }

        _thrusterSlider.value = _thrusterFuel;

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

        _playerControls = GetComponent<PlayerControls>();
        if (!_playerControls)
        {
            Debug.LogError("Player controls script not assigned");
        }

        _uiManager.SetMaxAmmo(_maxAmmo);
        _cameraShake.enabled = false;
        _rightEngineDamage.SetActive(false);
        _leftEngineDamage.SetActive(false);
        _thrusterYield = new WaitForSeconds(Time.deltaTime);
        _blinkYield = new WaitForSeconds(0.15f);
        _powerupYield = new WaitForSeconds(5.0f);
        _selectedWeaponPrefab = _laserPrefab;
    }

    void Update()
    {
        Movement();
        Thrusters();
        RoomWrap();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_playerControls.horizontalInput, _playerControls.verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
    }

    void Thrusters()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrusterFuel > 0)
        {
            _leftShift = !_leftShift;
            _originalSpeed = _speed;
            _newSpeed = _originalSpeed * 1.5f;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            _speed = _newSpeed;
            if (_thrusterFuel > 0)
            {
                _thrusterFuel -= Time.deltaTime;
                _thrusterSlider.value = _thrusterFuel;
            }

            if (_thrusterFuel <= 0)
            {
                _speed = _originalSpeed;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _leftShift = !_leftShift;
            _speed = _originalSpeed;
            StartCoroutine(ThrusterCooldownRoutine());
        }
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
            float yPosition = transform.position.y + 1.5f;
            Instantiate(_selectedWeaponPrefab, new Vector3(transform.position.x, yPosition, 0), Quaternion.identity);
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
            _spawnManager.OnPlayerDeathOrWin();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _uiManager.GameOver();
            Destroy(this.gameObject, 0.2f);
        }
    }

    public void HealPlayer()
    {
        if (_lives <= 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }
    }

    public void TripleShotActive()
    {
        _selectedWeaponPrefab = _tripleShotPrefab;
        StartCoroutine(AltWeaponPowerDownRoutine(5.0f));
    }

    public void HomingMissleActive()
    {
        _selectedWeaponPrefab = _homingMisslePrefab;
        StartCoroutine(AltWeaponPowerDownRoutine(5.0f));
    }

    public void SpreadShotActive()
    {
        _selectedWeaponPrefab = _spreadShotPrefab;
        StartCoroutine(AltWeaponPowerDownRoutine(10.0f));
    }

    IEnumerator AltWeaponPowerDownRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _selectedWeaponPrefab = _laserPrefab;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine ()
    {
        yield return _powerupYield;
        _speed /= _speedMultiplier; 
    }

    public void NegativePowerupActive()
    {
        _speed /= _speedMultiplier;
        StartCoroutine(SpeedDownPowerDownRoutine());
    }

    IEnumerator SpeedDownPowerDownRoutine()
    {
        yield return _powerupYield;
        _speed *= _speedMultiplier;
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
        if (_ammo + ammo > _maxAmmo)
        {
            _ammo = _maxAmmo;
        }
        else
        {
            _ammo += ammo;
        }
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
            yield return _blinkYield;
            _spriteRenderer.enabled = true;
            yield return _blinkYield;
        }
    }

    IEnumerator ThrusterCooldownRoutine()
    {
        while (_thrusterFuel <= 15 && !_leftShift)
        {
            _thrusterFuel += Time.deltaTime;
            _thrusterSlider.value = _thrusterFuel;
            yield return _thrusterYield;
        }
    }
}
