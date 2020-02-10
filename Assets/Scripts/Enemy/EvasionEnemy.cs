using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionEnemy : Enemy
{
    private Vector3 _direction;

    new void Start()
    {
        base.Start();
        _direction = Vector3.down;
    }

    new void Update()
    {
        base.Update();
    }

    public override void Movement()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    public void UpdateDirection(float force)
    {
        _direction = new Vector3(force, -1, 0);
    }
}
