using UnityEngine;

public class Pumpking : PumpkinBase
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        maxHealth = 12f;
        health = maxHealth;
        moveSpeed = 0.5f;
        attackDamage = 4f;
        attackSpeed = 2.5f;
    }
}
