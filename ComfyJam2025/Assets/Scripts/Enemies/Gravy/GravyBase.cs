using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GravyBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float healthDrain = 0.5f;
    [SerializeField] private float healRate = 1f;

    [SerializeField] private float auraSize = 4;
    private enum GravyState
    {
        Normal,
        Burn,
        Freeze,
    }

    [SerializeField] private float auraDuration = 5;

    private GravyState gravyState = GravyState.Normal;
    private SpriteRenderer aura;
    private float auraTimer = 0;

    protected Color BURN_AURA = new Color(0, 1, 0, 0.3f);
    protected Color FREEZE_AURA = new Color(0, 0, 0, 0.3f);
    protected Color EMPTY_AURA = new Color(0, 0, 0, 0);
    protected override void InitializeEnemy()
    {
        //Logger.Log($"Initializing {name}", LogLevel.debug);
        //maxHealth = 10;
        health = maxHealth;

        aura = transform.Find("AuraSprite").GetComponent<SpriteRenderer>();
        if (aura == null)
        {
            Logger.Log("Gravy enemy does not have aura sprite", LogLevel.fatal);
        }
        transform.Find("AuraSprite").localScale = new Vector3(auraSize * 2 - 0.25f,
            auraSize * 2 - 0.25f, 1);
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;

        switch (gravyState)
        {
            case GravyState.Normal:
                if (target != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        target.transform.position, moveSpeed * GameManager.GetDeltaTime());
                }
                aura.color = EMPTY_AURA;
                break;
            case GravyState.Burn:
                foreach (EnemyBase enemy in EnemyManager.GetEnemies())
                {
                    if (enemy == this) continue;
                    if (utils.FlatSqrDistance(GetPosition(), enemy.GetPosition()) < auraSize * auraSize)
                    {
                        enemy.TakeDamage(GameManager.GetDeltaTime() * -healRate);
                    }
                }

                // TODO: Heal mother
                TakeDamage(GameManager.GetDeltaTime() * healthDrain);
                aura.color = BURN_AURA;
                break;
            case GravyState.Freeze:
                foreach (EnemyBase enemy in EnemyManager.GetEnemies())
                {
                    if (enemy == this) continue;
                    float dist = utils.FlatSqrDistance(GetPosition(), enemy.GetPosition());
                    if (dist > 1 && dist < auraSize * auraSize)
                    {
                        enemy.SetPosition(utils.MoveTowardsOffset(enemy.GetPosition(), GetPosition(),
                            1, enemy.IsFrozen() ? 4f : 1.5f, GameManager.GetDeltaTime()));
                    }
                }
                aura.color = FREEZE_AURA;
                break;
        }

        auraTimer = Mathf.Max(0, auraTimer - GameManager.GetDeltaTime());
        if (auraTimer <= 0)
        {
            gravyState = GravyState.Normal;
        }
    }

    protected override void Update()
    {
        if (gravyState == GravyState.Burn)
        {
            tintColor.r = 0;
            tintColor.b = 0;
        }
        if (gravyState == GravyState.Freeze)
        {
            tintColor.r = 0;
            tintColor.b = 0;
            tintColor.g = 0;
        }
        base.Update();
    }

    public override void Freeze(float freezeDuration)
    {
        gravyState = GravyState.Freeze;
        auraTimer = auraDuration;
    }

    public override void Burn(float burnDuration)
    {
        gravyState = GravyState.Burn;
        auraTimer = auraDuration;
    }

//     protected override void Die()
//     {
//         // Find random drop
//         if (killingType != DamageType.Disintegrate && dropPrefabs.Count > 0)
//         {
//             Instantiate(dropPrefabs[(int)(UnityEngine.Random.value * dropPrefabs.Count)],
//                 transform.position, Quaternion.identity);
//         }
//         base.Die();
//     }
}
