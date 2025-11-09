using System.Collections.Generic;
using UnityEngine;


public class ZapSpell : SpellBase
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float snapDistance = 4;
    private Transform AimHighlight;
    private EnemyBase closestEnemy;
    public void Start()
    {
        AimHighlight = transform.Find("AimingCircle");
    }
    public override void Aim()
    {
        AimHighlight.position = GameManager.GetMousePos();

        closestEnemy = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), snapDistance);

        if (closestEnemy != null)
        {
            AimHighlight.position = closestEnemy.GetPosition();
        }
    }

    public override void Cast()
    {
        // Damage all enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, AimHighlight.position) < 81)
            {
                enemy.TakeDamage(damage);
                if (closestEnemy != null)
                {
                    enemy.CopyState(closestEnemy);
                }
            }
        }

        Destroy(gameObject);
    }
}