using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveDetection : MonoBehaviour
{
    private AggressiveEnemy _aggressiveEnemyParent;

    private void Start()
    {
        _aggressiveEnemyParent = GetComponentInParent(typeof(AggressiveEnemy)) as AggressiveEnemy;
        if (!_aggressiveEnemyParent)
        {
            Debug.LogError("Parent Smart Enemy not assigned in SmartDetection script");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Laser" && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
        {
            if (other.transform.position.x < _aggressiveEnemyParent.transform.position.x)
            {
                _aggressiveEnemyParent.UpdateDirection(0.6f);
            }
            else
            {
                _aggressiveEnemyParent.UpdateDirection(-0.6f);
            }
        }

        if (other.tag == "Player")
        {
            if (other.transform.position.x < _aggressiveEnemyParent.transform.position.x)
            {
                _aggressiveEnemyParent.UpdateDirection(-0.8f);
            }
            else
            {
                _aggressiveEnemyParent.UpdateDirection(0.8f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laser" && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
            _aggressiveEnemyParent.UpdateDirection(0.0f);
        if (other.tag == "Player")
            _aggressiveEnemyParent.UpdateDirection(0.0f);
    }
}
