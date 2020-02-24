using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool _gameOver;
    private bool _waveOver;
    // handle to text
    [SerializeField]
    private Text _scoreText;
    private int _score = 0;
    [SerializeField]
    private Text _ammoText;
    private int _ammo = 15;
    private int _maxAmmo = 60;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _waveText;
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _livesImage.sprite = _livesSprites[3];
        _gameOverText.gameObject.SetActive(false);
        _gameOver = false;
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _waveText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.text = "Score: " + _score;
        _ammoText.text = "Ammo: " + _ammo + "/" + _maxAmmo;
    }

    public void UpdateScore(int score)
    {
        _score = score;
    }

    public void UpdateAmmo(int ammo)
    {
        _ammo = ammo;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];
    }

    public void GameOver()
    {
        _gameOver = true;
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickerRoutine());
    }

    public void OnWaveOver()
    {
        _waveOver = true;
        StartCoroutine(WaveOverFlickerRoutine());
    }

    public void SetWaveStart()
    {
        _waveOver = false;
        _waveText.gameObject.SetActive(false);
    }

    public void SetMaxAmmo(int ammo)
    {
        _maxAmmo = ammo;
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(_gameOver)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WaveOverFlickerRoutine()
    {
        while(_waveOver)
        {
            _waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _waveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
