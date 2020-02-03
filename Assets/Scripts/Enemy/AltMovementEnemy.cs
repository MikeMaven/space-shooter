using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltMovementEnemy : Enemy
{
    private WaitForSeconds _movementYield;
    private Vector3 _direction = Vector3.down;

    private void Awake()
    {
        _movementYield = new WaitForSeconds(1.0f);
    }

    public override void Movement()
    {
        StartCoroutine(MovementCoroutine());
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    IEnumerator MovementCoroutine()
    {
        while(true)
        {
            _direction = Vector3.down;
            yield return _movementYield;
            _direction = Vector3.left;
            yield return _movementYield;
            _direction = Vector3.down;
            yield return _movementYield;
            _direction = Vector3.right;
            yield return _movementYield;
        }
    }
}
