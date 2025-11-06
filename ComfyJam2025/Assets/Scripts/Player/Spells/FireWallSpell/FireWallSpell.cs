using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FireWallSpell : SpellBase
{

    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingRect");
    }
    public override void Aim()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        AimHighlight.position = mousePos;
        AimHighlight.rotation = Quaternion.Euler(Vector3.forward *
            Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg);
    }

    public override void Cast()
    {
        List<EnemyBase> hitEnemies = new List<EnemyBase>();
        // Damage all enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, GameManager.GetMousePos()) < 25)
            {
                hitEnemies.Add(enemy);
            }
        }

        // TODO: Fix this workaround for affecting hashset during iteration
        foreach (EnemyBase enemy in hitEnemies)
        {
            enemy.TakeDamage(5);
        }
        Destroy(gameObject);
    }
}