using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltMovementEnemy : Enemy
{
    private WaitForSeconds _movementYield;
    [SerializeField]
    private Vector3 _direction;

    private void Start()
    {
        _movementYield = new WaitForSeconds(1.0f);
        StartCoroutine(MovementCoroutine());
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("The player is not assigned in the enemy script");
        }

        _anim = GetComponent<Animator>();
        if (!_anim)
        {
            Debug.LogError("The animator component is not present");
        }

        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.LogError("The enemy audio source is not assigned");
        }
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
