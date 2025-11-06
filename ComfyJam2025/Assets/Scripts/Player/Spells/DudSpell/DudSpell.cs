using System.Collections.Generic;
using UnityEngine;


public class DudSpell : SpellBase
{

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
            if (utils.FlatSqrDistance(enemy.transform.position, GameManager.GetMousePos()) < 16)
            {
                hitEnemies.Add(enemy);
            }
        }

        // TODO: Fix this workaround for affecting hashset during iteration
        foreach (EnemyBase enemy in hitEnemies)
        {
            enemy.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}