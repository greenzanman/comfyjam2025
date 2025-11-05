using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;

    public List<GameObject> dropPrefabs;

   
    protected override void InitializeEnemy()
    {
        Logger.Log($"Initializing {name}", LogLevel.debug);
        //maxHealth = 10;
        health = maxHealth;
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target.transform.position, moveSpeed * GameManager.GetDeltaTime());
        }
    }

    protected override void Die()
    {
        // Find random drop
        if (dropPrefabs.Count > 0)
        {
            Instantiate(dropPrefabs[(int)(UnityEngine.Random.value * dropPrefabs.Count)],
                transform.position, Quaternion.identity);
        }
        base.Die();
    }
}
