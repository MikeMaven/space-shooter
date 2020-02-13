using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltMovementEnemy : Enemy
{
    private WaitForSeconds _movementYield;
    [SerializeField]
    private Vector3 _direction;

    new void Start()
    {
        base.Start();
        _movementYield = new WaitForSeconds(2.0f);
        StartCoroutine(MovementCoroutine());
    }

    public override void Movement()
    {
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
