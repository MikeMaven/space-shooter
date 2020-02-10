using UnityEngine;
using System.Collections;

public class SmartEnemy : Enemy
{
    private Vector3 _direction;
    private bool _canFireBackwards;

    new void Start()
    {
        base.Start();
        _direction = Vector3.down;
        _canFireBackwards = true;
    }

    new void Update()
    {
        base.Update();
        if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up), 5, 1<<9) && _canFireBackwards)
        {
            StartCoroutine(BackwardsLaserCooldown());
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
                lasers[i].AssignReverseLaser();
            }
        }
    }

    public override void Movement()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    public void UpdateDirection(float force)
    {
        _direction = new Vector3(force, -1, 0);
    }

    IEnumerator BackwardsLaserCooldown()
    {
        _canFireBackwards = false;
        yield return new WaitForSeconds(3.0f);
        _canFireBackwards = true;
    }
}
