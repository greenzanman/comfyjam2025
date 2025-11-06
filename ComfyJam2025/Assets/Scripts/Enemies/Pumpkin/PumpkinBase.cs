using System.Collections.Generic;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    private float moveSpeed = 1f;

    [SerializeField]
    private float attackRange = 0.5f;   // distance to start attacking
    
    [SerializeField]
    private float attackSpeed = 2.0f;   // seconds between attacks
    
    [SerializeField]
    private float attackDamage = 1f;    // damage dealt each hit
    
    private float attackTimer = 0f;     // cooldown timer

    public List<GameObject> dropPrefabs;

    protected override void InitializeEnemy()
    {
        Logger.Log($"Initializing {name}", LogLevel.debug);
        health = maxHealth;
    }

    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;
        if (target == null) return;

        // Find distance to player
        Vector3 playerPos = PlayerManager.instance.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);

        // Check if within attack range
        if (distance <= attackRange)
        {
            // Stop moving
            // Face player (optional)
            // transform.LookAt(playerPos);

            // Handle attack cooldown
            attackTimer += GameManager.GetDeltaTime();
            if (attackTimer >= attackSpeed)
            {
                AttackPlayer();
                attackTimer = 0f;
            }
        }
        else
        {
            // Move toward target normally
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                moveSpeed * GameManager.GetDeltaTime());

            // Reset attack timer so it doesn't "store up" attacks
            attackTimer = 0f;
        }
    }

    private void AttackPlayer()
    {
        if (PlayerManager.instance == null) return;

        PlayerManager.instance.TakeDamage(attackDamage);
        Logger.Log($"Pumpkin attacked! Player health: {PlayerManager.instance.health}", LogLevel.info);
    }

    protected override void Die()
    {
        if (dropPrefabs.Count > 0)
        {
            Instantiate(
                dropPrefabs[(int)(UnityEngine.Random.value * dropPrefabs.Count)],
                transform.position,
                Quaternion.identity);
        }
        base.Die();
    }
}
