using System.Collections.Generic;
using UnityEngine;


public class ZapSpell : SpellBase
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float snapDistance = 4;
    [SerializeField] private AudioSource _zappySource;
    private Transform AimHighlight;
    private EnemyBase closestEnemy;

    private List<EnemyBase> enemiesInRange = new List<EnemyBase>();

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
        // Tell AudioManager to play spell sfx
        AudioManager.instance.PlaySpellSound(SpellSound.Zappy);


        // Damage all enemies
        
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (utils.FlatSqrDistance(enemy.transform.position, AimHighlight.position) < 81)
            {
                enemiesInRange.Add(enemy);
                if (closestEnemy != null)
                {
                    enemy.CopyState(closestEnemy);
                }
            }
        }

        ChainLightningVFX chainLightningVFX = FindObjectOfType<ChainLightningVFX>();
        chainLightningVFX.ActivateChain(enemiesInRange);
        foreach (EnemyBase enemy in enemiesInRange) {
            enemy.TakeDamage(damage);
        }

        delayedDeath.StartDelayedDeath();
    }
}