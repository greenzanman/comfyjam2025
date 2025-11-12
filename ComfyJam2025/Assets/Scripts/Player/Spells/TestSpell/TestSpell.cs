using System.Collections.Generic;
using UnityEngine;


public class TestSpell : SpellBase
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
        Logger.Log("Casting TestSpell", LogLevel.debug);

        // Damage all enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, GameManager.GetMousePos()) < 25)
            {
                enemy.TakeDamage(5);
            }
        }

        Destroy(gameObject);
    }
}