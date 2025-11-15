using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FireConeSpell : SpellBase
{
    private PolygonCollider2D coneCollider;
    [SerializeField] private float damage = 5;

    public void Start()
    {
        coneCollider = GetComponentInChildren<PolygonCollider2D>();
    }
    public override void Aim()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        float ang = Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg;
        if (mousePos.x < 0)
        {
            ang += 180;
        }
        transform.rotation = Quaternion.Euler(Vector3.forward *
            ang);

    }

    public override void Cast()
    {
        // Tell AudioManager to play spell sfx
        AudioManager.instance.PlaySpellSound(SpellSound.FireBoom);

        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (coneCollider.OverlapPoint(enemy.GetPosition()))
            {
                PlayVFX(enemy.transform.position);
                enemy.TakeDamage(damage, DamageType.Fire);
            }
        }
        delayedDeath.StartDelayedDeath();
    }
}