using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class EnemyBase : MonoBehaviour
{
    public float health { get; protected set; }

    [field: SerializeField] protected float maxHealth = 4;

    private CenterStation centerStation;

    private Color HEALTH_COLOR = Color.red;

    [Header("DROPS")]
    [SerializeField] private GameObject itemDropBasePrefab;
    public List<ItemData> possibleDrops;

    private void Start()
    {
        // Register with manager
        EnemyManager.RegisterEnemy(this);

        // Initialize values
        InitializeEnemy();
    }

    protected virtual void InitializeEnemy()
    {
        Logger.Log($"InitializeEnemy not implemented for {name}", LogLevel.error);
        return;
    }

    private void Update()
    {
        Think();
    }
    public virtual void TakeDamage(float damageAmount, DamageType damageType = DamageType.None)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    // Contains majority of ai thoughts for a given enemy, overrided for a given unit
    protected virtual void Think()
    {
        Logger.Log($"Think not implemented for {name}", LogLevel.error);
    }

    protected virtual void Die()
    {
        // Find random drop
        if (possibleDrops.Count > 0) {
            DetermineDroppedItem();
        }

        // Inform manager
        EnemyManager.DeregisterEnemy(this);

        Destroy(gameObject);
    }
    private void DetermineDroppedItem() {
        float weight = CalculateDropWeight(out ItemData maximumChanceItem);
        Random.InitState(Random.Range(1000, 9999));
        float chance = Random.Range(0f, weight);
        ItemData selectedItem = maximumChanceItem;

        foreach (ItemData item in possibleDrops) {
            float dropChance = item.dropChance;
            Debug.Log($"chance: {chance} > weight: {weight} | dropped: {dropChance}");
            if (chance < dropChance) {
                selectedItem = item;
                break;
            }
            chance -= dropChance;
        }

        // if no item selected, drop highest chance
        GameObject drop = Instantiate(itemDropBasePrefab, transform.position, Quaternion.identity);
        drop.GetComponentInChildren<SpriteRenderer>().sprite = selectedItem.itemSprite;
        drop.GetComponent<EnemyDropBase>().itemType = selectedItem.itemType;
    }
    private float CalculateDropWeight(out ItemData maximumChanceItem) {
        float weight = 0f;
        maximumChanceItem = possibleDrops[0];
        foreach (ItemData itemData in possibleDrops) {
            if (itemData.dropChance > maximumChanceItem.dropChance) {
                maximumChanceItem = itemData;
            }
            weight += itemData.dropChance;
        }

        return weight;
    }

    private void OnGUI()
    {
        if (DebugManager.GetConsoleVar("DrawEnemyHealth") == 1)
        { 
            // TODO: Fix this weird workaround
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                new Vector3(transform.position.x, -transform.position.y, 0));
            EditorGUI.DrawRect(new Rect(screenPos.x - 30, screenPos.y - 50,
                60 * health / maxHealth, 10), HEALTH_COLOR);
        }
    }
}
