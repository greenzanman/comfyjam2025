using System.Collections.Generic;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    [Header("Movement & Combat")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float attackRange = 0.5f;
    [SerializeField] protected float attackSpeed = 2.0f;
    [SerializeField] protected float attackDamage = 1f;

    protected float attackTimer = 0f;
    protected Rigidbody2D rb;

    [Header("Drops")]
    public List<GameObject> dropPrefabs;

    protected override void InitializeEnemy()
    {
        Logger.Log($"Initializing {name}", LogLevel.debug);
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;
        if (target == null) return;

        float dt = GameManager.GetDeltaTime();

        Vector2 targetPos = target.transform.position;
        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance <= attackRange)
        {
            attackTimer += dt;
            if (attackTimer >= attackSpeed)
            {
                AttackPlayer();
                attackTimer = 0f;
            }
        }
        else
        {
            MoveTowardTarget(targetPos, dt);
        }
    }

    protected virtual void MoveTowardTarget(Vector2 targetPos, float dt)
    {
        Vector2 pos = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * dt);
        if (rb) rb.MovePosition(pos);
        else transform.position = pos;
    }
 
    protected virtual void AttackPlayer() 
    { 
        if (PlayerManager.instance == null) return; 
        
        PlayerManager.instance.TakeDamage(attackDamage); 
        Logger.Log($"Pumpkin attacked! Player health: {PlayerManager.instance.health}", LogLevel.info); 
    }

    protected override void Die()
    {
        if (dropPrefabs != null && dropPrefabs.Count > 0)
        {
            Instantiate(
                dropPrefabs[Random.Range(0, dropPrefabs.Count)],
                transform.position,
                Quaternion.identity);
        }
        
        base.Die();
    }
}
