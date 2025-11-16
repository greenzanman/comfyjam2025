using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GravyBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float healthDrain = 0.5f;
    [SerializeField] private float healRate = 1f;

    [SerializeField] private float auraSize = 4;
    [SerializeField] private List<Sprite> sprites;
    private enum GravyState
    {
        Normal,
        Burn,
        Freeze,
    }

    [SerializeField] private float auraDuration = 5;

    private GravyState gravyState = GravyState.Normal;
    private SpriteRenderer aura;
    private SpriteRenderer mainSprite;
    private float auraTimer = 0;
    private float spriteTimer = 0;

    protected Color BURN_AURA = new Color(0, 1, 0, 0.05f);
    protected Color FREEZE_AURA = new Color(0, 0, 0, 0.05f);
    protected Color EMPTY_AURA = new Color(0, 0, 0, 0);
    protected override void InitializeEnemy()
    {
        health = maxHealth;

        mainSprite = transform.Find("Visual").GetComponent<SpriteRenderer>();

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

        mainSprite.flipX = GetPosition().x < target.transform.position.x;
        spriteTimer += GameManager.GetDeltaTime();

        switch (gravyState)
        {
            case GravyState.Normal:
                if (target != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        target.transform.position, moveSpeed * GameManager.GetDeltaTime());
                }
                aura.color = EMPTY_AURA;

                mainSprite.sprite = sprites[(int)(spriteTimer * 10) % 5 + 8];
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
                mainSprite.sprite = sprites[(int)(spriteTimer * 10) % 8 + 13];

                // TODO: Heal mother
                if (target != null)
                    if (utils.FlatSqrDistance(GetPosition(), target.transform.position) < auraSize * auraSize)
                        PlayerManager.instance.TakeDamage(GameManager.GetDeltaTime() * -healRate);
                        
                TakeDamage(GameManager.GetDeltaTime() * healthDrain);
                aura.color = BURN_AURA;
                break;
            case GravyState.Freeze:
                foreach (EnemyBase enemy in EnemyManager.GetEnemies())
                {
                    // Gravy monsters aren't healed by this
                    if (enemy is GravyBase) continue;
                    float dist = utils.FlatSqrDistance(GetPosition(), enemy.GetPosition());
                    if (dist > 1 && dist < auraSize * auraSize)
                    {
                        enemy.SetPosition(utils.MoveTowardsOffset(enemy.GetPosition(), GetPosition(),
                            1, enemy.IsFrozen() ? 4f : 1.5f, GameManager.GetDeltaTime()));
                    }
                }
                if (target != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        target.transform.position, moveSpeed / 6 * GameManager.GetDeltaTime());
                }
                mainSprite.sprite = sprites[(int)(spriteTimer * 10) % 8];
                aura.color = FREEZE_AURA;
                break;
        }

        auraTimer = Mathf.Max(0, auraTimer - GameManager.GetDeltaTime());
        if (auraTimer <= 0)
        {
            gravyState = GravyState.Normal;
        }

        if (target != null)
        {
            if (utils.FlatSqrDistance(GetPosition(), target.transform.position) <= contactRange * contactRange)
            {
                DealPlayerDamage(contactDamage);
                return;
            }
        }
    }

    // Does not use renderer color
    public override void SetRendererColor( Color color )
    {
        return;
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

}
