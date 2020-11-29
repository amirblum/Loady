using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask keyLayerMask;
    [SerializeField] private AudioEvent pickupKeyAudioEvent;

    private PlayerMovement movement;
    
    public bool HasKey { get; set; }

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        HasKey = false;
    }

    private void FixedUpdate()
    {
        EnemyCheck();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & keyLayerMask.value) > 0)
        {
            pickupKeyAudioEvent.Play();
            HasKey = true;
            Destroy(other.gameObject);
        }
    }

    private void EnemyCheck()
    {
        RaycastHit2D hit = movement.HitBelow(enemyLayerMask);
        if (hit.collider && movement.velocity.y < -0.01f)
        {
            var enemy = hit.collider.GetComponent<EnemyScript>();
            if (enemy.Alive)
            {
                enemy.KillEnemy();
                movement.MiniJump();
            }
        }
    }
}
