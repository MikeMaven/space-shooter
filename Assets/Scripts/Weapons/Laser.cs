using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    public float speed = 8.0f;
    public bool isEnemyLaser;
    public bool isReverseLaser;

    void Update()
    {
        if (!isEnemyLaser || isReverseLaser)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public virtual void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        isEnemyLaser = true;
    }

    public void AssignReverseLaser()
    {
        isReverseLaser = true;
    }

    void  OnTriggerEnter2D(Collider2D other)    
    {
        if (other.tag == "Player" && isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if (player)
            {
                player.LoseALife();
            }
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
