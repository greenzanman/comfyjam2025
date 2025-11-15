using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pumpking : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float range = 2f;
    [SerializeField] private List<Sprite> sprites;
    private SpriteRenderer spriteRenderer;
    
    private float spriteTimer = 0;

    protected override void InitializeEnemy()
    {
        health = maxHealth;

        spriteRenderer = transform.Find("Visual").GetComponent<SpriteRenderer>();
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;
        spriteTimer += GameManager.GetDeltaTime();

        if (Vector3.Distance(transform.position, target.transform.position) <= range) return;

        if (target != null)
        {
            int spriteIndex = (int)(spriteTimer * 5) % 9;

            if (spriteIndex >=4 && spriteIndex <= 8)
                transform.position = Vector3.MoveTowards(transform.position,
                    target.transform.position, moveSpeed * 2 * GameManager.GetDeltaTime());

            spriteRenderer.sprite = sprites[spriteIndex];
            spriteRenderer.flipX = GetPosition().x > target.transform.position.x;
        }

    }

    public override void TakeDamage(float damageAmount, DamageType damageType = DamageType.None)
    {
        // Every 8 damage, drop something
        float oldHealth = health;

        base.TakeDamage(damageAmount, damageType);

        if ((int)(oldHealth / 8) != (int)(health / 8))
        {
            DetermineDroppedItem();
        }
    }

    protected override void Die()
    {
        base.Die();
    }
}
