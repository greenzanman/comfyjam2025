using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombawk : EnemyBase
{
    [Header("Boombawk Settings")]
    [SerializeField] private float moveSpeed = 0.5f;      // creeping speed
    [SerializeField] private float boomDamage = 10f;
    [SerializeField] private float boomRadius = 5f;       // tweak to 1/3 screen width
    [SerializeField] private float fuseTime = 3f;         // countdown once lit
    [SerializeField] private List<Sprite> sprites;         // countdown once lit

    private enum BoombawkState { Creeping, FuseLit, Exploded }
    private BoombawkState state = BoombawkState.Creeping;

    private float spriteTimer = 0;

    private SpriteRenderer mainSprite;
    private Transform auraTransform;
    private SpriteRenderer previewAuraSprite;
    private float fuseTimer;

    protected override void InitializeEnemy()
    {
        Logger.Log($"Initializing Boombawk", LogLevel.debug);
        health = maxHealth;


        mainSprite = transform.Find("Visual").GetComponent<SpriteRenderer>();

        auraTransform = transform.Find("PreviewAura");
        previewAuraSprite = auraTransform.GetComponent<SpriteRenderer>();
        auraTransform.transform.localScale = new Vector3(boomRadius, boomRadius * 0.5f, 1);
    }

    protected override void Think()
    {
        spriteTimer += GameManager.GetDeltaTime();

        switch (state)
        {
            case BoombawkState.Creeping:
                Creeping();
                break;

            case BoombawkState.FuseLit:
                FuseLit();
                break;

            case BoombawkState.Exploded:
                Exploded();
                break;
        }
    }

    private void Creeping()
    {
        CenterStation target = GameManager.centerStation;
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target.transform.position, moveSpeed * GameManager.GetDeltaTime());
        
            mainSprite.flipX = GetPosition().x < target.transform.position.x;
        }

        mainSprite.sprite = sprites[(int)(spriteTimer * 10) % 6];

        SetPreviewVisual(Color.yellow, 0.3f);
    }

    private void FuseLit()
    {
        fuseTimer -= GameManager.GetDeltaTime();
        mainSprite.sprite = sprites[(int)(spriteTimer * 10) % 6 + 6];
        SetPreviewVisual(Color.red, fuseTimer % 0.6f > 0.3f ? 0 : 0.3f);
        
        CenterStation target = GameManager.centerStation;
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target.transform.position, moveSpeed / 6 * GameManager.GetDeltaTime());
        
            mainSprite.flipX = GetPosition().x < target.transform.position.x;
        }

        if (fuseTimer <= 0)
        {
            Explode();
        }
    }

    private void Exploded()
    {
        mainSprite.sprite = sprites[Mathf.Min((int)(spriteTimer * 10) + 12, 17)];
        fuseTimer -= GameManager.GetDeltaTime();
        if (fuseTimer <= 0)
        {
            Die();
        }
    }

    private void Explode()
    {
        state = BoombawkState.Exploded;

        // Damage nearby enemies (except itself)
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.GetPosition(), GetPosition()) < boomRadius * boomRadius )
                if (enemy != this)
                    enemy.TakeDamage(boomDamage);
        }

        // Check damage to Mama (CenterStation)
        if (GameManager.centerStation != null)
        {
            if (utils.FlatSqrDistance(GameManager.centerStation.transform.position, GetPosition()) < boomRadius * boomRadius )
            {
                // PlayerManager.instance.TakeDamage(boomDamage);
                Logger.Log($"BOOMBAWK hit Mama! {boomDamage} damage dealt.", LogLevel.info);
            }
        }

        spriteTimer = 0;
        state = BoombawkState.Exploded;
        fuseTimer = 0.4f;
        SetPreviewVisual(Color.white, 0);
    }

    public override void SetRendererColor( Color color )
    {
        mainSprite.color = color;
    }

    public override void Burn(float burnDuration)
    {
        base.Burn(burnDuration);
        if (state == BoombawkState.Creeping)
        {
            // Light fuse
            state = BoombawkState.FuseLit;
            fuseTimer = fuseTime;
        }
    }

    public override void Freeze(float freezeDuration)
    {
        base.Freeze(freezeDuration);

        if (state == BoombawkState.FuseLit)
        {
            // Defuse
            state = BoombawkState.Creeping;
            fuseTimer = 0;
            SetPreviewVisual(Color.yellow, 0.3f);
        }
    }

    private void SetPreviewVisual(Color color, float alpha)
    {
        previewAuraSprite.color = new Color(color.r, color.g, color.b, alpha);
    }
}