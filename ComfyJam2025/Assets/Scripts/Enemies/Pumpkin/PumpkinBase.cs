using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PumpkinBase : EnemyBase
{
    [SerializeField] private float moveSpeed = 0.5f;

    [SerializeField] private GameObject itemDropBasePrefab;
    public List<ItemData> possibleDrops;
   
    protected override void InitializeEnemy()
    {
        //Logger.Log($"Initializing {name}", LogLevel.debug);
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
        if (possibleDrops.Count > 0)
        {
            ItemData selectedItem = possibleDrops[(int)(UnityEngine.Random.value * possibleDrops.Count)];
            GameObject drop = Instantiate(itemDropBasePrefab, transform.position, Quaternion.identity);
            drop.GetComponentInChildren<SpriteRenderer>().sprite = selectedItem.itemSprite;
            drop.GetComponent<EnemyDropBase>().itemType = selectedItem.itemType;
        }
        base.Die();
    }
}
