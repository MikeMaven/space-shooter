using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private int _numberOfWaves = 10;
    [SerializeField]
    private int _wave = 1;
    [SerializeField]
    private float _waveMultipler = 1.5f;
    [SerializeField]
    private float _numberOfEnemiesInWave = 10.0f;
    private float _previousWaveEnemies;
    private float _enemiesToKillInWave;
    public enum WaveDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    public WaveDifficulty waveDifficulty;
    [SerializeField]
    private GameObject[] _allEnemyPrefabs;
    private List<GameObject> _waveEnemies = new List<GameObject>();
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning;
    [SerializeField]
    private float _spawnFrequency = 2.0f;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private GameObject _ammoRefill;
    [SerializeField]
    private GameObject _spreadShot;
    [SerializeField]
    private GameObject _boss;

    public bool StopSpawning => _stopSpawning;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnAmmoRoutine());
        StartCoroutine(SpawnRarePowerupsRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        _previousWaveEnemies = _numberOfEnemiesInWave;
        _enemiesToKillInWave = _numberOfEnemiesInWave;
        SetWaveDifficulty(_wave);
        SetEnemiesForWave();
        yield return new WaitForSeconds(2.0f);
        while (!_stopSpawning && _numberOfEnemiesInWave >= 0)
        {
            GameObject _enemyPrefab = _waveEnemies[Random.Range(0, _waveEnemies.Count)];
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            GameObject spawn = Instantiate(_enemyPrefab, randomPosAboveScreen, Quaternion.identity);
            spawn.transform.parent = _enemyContainer.transform;
            _numberOfEnemiesInWave--;
            yield return new WaitForSeconds(_spawnFrequency);
        }
        yield return new WaitUntil(() => _enemiesToKillInWave <= 0);
        if (_wave < _numberOfWaves)
        {
            StartCoroutine(WaveCooldown());
        }
        else
        {
            _boss.SetActive(true);
        }
    }

    IEnumerator WaveCooldown()
    {
        _stopSpawning = true;
        _wave++;
        _numberOfEnemiesInWave = Mathf.Ceil(_previousWaveEnemies * _waveMultipler);
        yield return new WaitForSeconds(5.0f);
        _stopSpawning = false;
        StartSpawning();
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        while (!_stopSpawning)
        {
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            int randomPowerup = Random.Range(0, _powerups.Length);
            Instantiate(_powerups[randomPowerup], randomPosAboveScreen, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        }
    }

    IEnumerator SpawnAmmoRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        while (!_stopSpawning)
        {
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            Instantiate(_ammoRefill, randomPosAboveScreen, Quaternion.identity);
            yield return new WaitForSeconds(8.0f);
        }
    }

    IEnumerator SpawnRarePowerupsRoutine()
    {
        yield return new WaitForSeconds(30.0f);
        while (!_stopSpawning)
        {
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            int randomPowerup = Random.Range(0, _rarePowerups.Length);
            Instantiate(_rarePowerups[randomPowerup], randomPosAboveScreen, Quaternion.identity);
            yield return new WaitForSeconds(30.0f);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    private void SetWaveDifficulty(int wave)
    {
        if (wave > 0 && wave < Mathf.Ceil(_numberOfWaves / 3))
        {
            waveDifficulty = WaveDifficulty.Easy;
        }
        else if (wave >= 4 && wave < Mathf.Ceil((_numberOfWaves / 3) * 2))
        {
            waveDifficulty = WaveDifficulty.Medium;
        }
        else
        {
            waveDifficulty = WaveDifficulty.Hard;
        }
    }

    public void OnEnemyDeath()
    {
        _enemiesToKillInWave--;
    }

    private void SetEnemiesForWave()
    {
        switch (waveDifficulty)
        {
            case WaveDifficulty.Easy:
                foreach(GameObject enemy in _allEnemyPrefabs)
                {
                    if (enemy.GetComponent<Enemy>().enemyType == Enemy.EnemyType.Easy)
                    {
                        _waveEnemies.Add(enemy);
                    }
                }
                break;
            case WaveDifficulty.Medium:
                foreach (GameObject enemy in _allEnemyPrefabs)
                {
                    if (enemy.GetComponent<Enemy>().enemyType == Enemy.EnemyType.Medium)
                    {
                        _waveEnemies.Add(enemy);
                    }
                }
                break;
            case WaveDifficulty.Hard:
                foreach (GameObject enemy in _allEnemyPrefabs)
                {
                    if (enemy.GetComponent<Enemy>().enemyType == Enemy.EnemyType.Hard)
                    {
                        _waveEnemies.Add(enemy);
                    }
                }
                break;
        }
    }
}
