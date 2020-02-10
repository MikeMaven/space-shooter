using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartDetection : MonoBehaviour
{
    private SmartEnemy _smartEnemyParent;

    private void Start()
    {
        _smartEnemyParent = GetComponentInParent(typeof(SmartEnemy)) as SmartEnemy;
        if(!_smartEnemyParent)
        {
            Debug.LogError("Parent Smart Enemy not assigned in SmartDetection script");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
        {
            if (other.transform.position.x < _smartEnemyParent.transform.position.x)
            {
                _smartEnemyParent.UpdateDirection(0.6f);
            }
            else
            {
                _smartEnemyParent.UpdateDirection(-0.6f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laser" && !other.gameObject.GetComponent<Laser>().isEnemyLaser)
            _smartEnemyParent.UpdateDirection(0.0f);
    }
}
