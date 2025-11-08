using System.Collections.Generic;
using UnityEngine;


public class FreezeSpell : SpellBase
{
    private const float DAMAGE_RADIUS = 5;
    [SerializeField] private float freezeDuration = 8;
    [SerializeField] private float damage = 2;
    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingCircle");
    }
    public override void Aim()
    {
        AimHighlight.position = GameManager.GetMousePos();
    }

    public override void Cast()
    {
        List<EnemyBase> hitEnemies = new List<EnemyBase>();
        // Damage all enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, GameManager.GetMousePos()) < DAMAGE_RADIUS * DAMAGE_RADIUS)
            {
                hitEnemies.Add(enemy);
            }
        }

        // TODO: Fix this workaround for affecting hashset during iteration
        foreach (EnemyBase enemy in hitEnemies)
        {
            enemy.TakeDamage(damage);
            enemy.Freeze(freezeDuration);
        }
        Destroy(gameObject);
    }
}