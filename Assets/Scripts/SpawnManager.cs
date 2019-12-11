using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning;
    [SerializeField]
    private float _spawnFrequency = 2.0f;
    [SerializeField]
    private GameObject[] _powerups;

    public bool StopSpawning => _stopSpawning;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        while (!_stopSpawning)
        {
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            GameObject spawn = Instantiate(_enemyPrefab, randomPosAboveScreen, Quaternion.identity);
            spawn.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnFrequency);
        }

    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        while(_stopSpawning == false)
        {
            Vector3 randomPosAboveScreen = new Vector3(Random.Range(-9.4f, 9.4f), 6.05f, 0);
            int randomPowerup = Random.Range(0, 3);
            Instantiate(_powerups[randomPowerup], randomPosAboveScreen, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(8.0f, 12.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
