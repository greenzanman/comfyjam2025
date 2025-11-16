using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;

    private SpriteRenderer spriteRenderer;

    protected override void InitializeEnemy()
    {
        health = maxHealth;

        spriteRenderer = transform.Find("Visual").GetComponent<SpriteRenderer>();
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;


        if (target != null)
        {
            
            if (utils.FlatSqrDistance(GetPosition(), target.transform.position) <= contactRange * contactRange)
            {
                DealPlayerDamage(contactDamage);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position,
                target.transform.position, moveSpeed * GameManager.GetDeltaTime());

        
            spriteRenderer.flipX = GetPosition().x > target.transform.position.x;
        }

    }

    protected override void Die()
    {
        base.Die();
    }
}
