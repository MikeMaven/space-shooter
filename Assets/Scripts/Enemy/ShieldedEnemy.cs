using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : Enemy
{
    [SerializeField]
    private GameObject _shieldsPrefab;
    new void Start()
    {
        base.Start();
        SetShield(true);
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if(other.tag == "Player" || other.tag == "Laser")
        {
            SetShield(false);
            _shieldsPrefab.SetActive(false);
        }
    }
}
