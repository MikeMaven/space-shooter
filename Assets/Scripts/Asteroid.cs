using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _rotationSpeed = 19.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            if(_explosionPrefab && _spawnManager){
                Destroy(other.gameObject);
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                _spawnManager.StartSpawning();
                Destroy(this.gameObject, 0.2f);
            }
        }
    }
}
