using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float range = 2f;

    private SpriteRenderer spriteRenderer;

    protected override void InitializeEnemy()
    {
        health = maxHealth;

        spriteRenderer = transform.Find("Visual").GetComponent<SpriteRenderer>();
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;

        if (Vector3.Distance(transform.position, target.transform.position) <= range) return;

        if (target != null)
        {
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
