using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FireConeSpell : SpellBase
{
    private PolygonCollider2D coneCollider;
    private Transform AimHighlight;
    [SerializeField] private float damage = 5;
    public void Start()
    {
        AimHighlight = transform.Find("AimingRect");

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
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (coneCollider.OverlapPoint(enemy.GetPosition()))
            {
                enemy.TakeDamage(damage, DamageType.Fire);
            }
        }
        // Create firewall
        Destroy(gameObject);
    }
}