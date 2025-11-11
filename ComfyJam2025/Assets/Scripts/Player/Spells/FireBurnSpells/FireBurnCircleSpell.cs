using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


public class FireBurnCircleSpell : SpellBase
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float burnDuration = 6;
    [SerializeField] private float radius = 7;
    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingCircle");

        AimHighlight.localScale = new Vector3(2 * radius - 0.5f, 2 * radius - 0.5f, 1);
    }
    public override void Aim()
    {
        AimHighlight.position = GameManager.GetMousePos();
    }

    public override void Cast()
    {
        // Damage all enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, GameManager.GetMousePos()) < radius * radius)
            {
                enemy.TakeDamage(damage);
                enemy.Burn(burnDuration);
            }
        }

        Destroy(gameObject);
    }
}