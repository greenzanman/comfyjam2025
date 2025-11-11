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
    protected DamageType killingType = DamageType.None;

    private CenterStation centerStation;

    private Color HEALTH_COLOR = new Color(1, 0, 0, 0.5f);
    private Color POS_COLOR = new Color(0, 0, 1, 0.3f);
    private Color tintColor;
    private Color currentTint = Color.white;
    private float freezeTimer = 0;
    private const float MELT_DAMAGE = 2;
    private Vector2 windDirection;
    private float windDuration;
    private const float WIND_RATIO = 9;
    private float burnTimer = 0;
    [SerializeField] private float density = 1;

    [Header("DROPS")]
    [SerializeField] private GameObject itemDropBasePrefab;
    public List<ItemData> possibleDrops;

    private bool isDead = false;
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
        tintColor = Color.white;
        if (windDuration > 0)
        {
            windDuration -= GameManager.GetDeltaTime();
            Vector2 windForce = windDirection * GameManager.GetDeltaTime() / density * WIND_RATIO;
            if (freezeTimer > 0)
                windForce /= 8;
            transform.position += new Vector3(windForce.x, windForce.y, 0);
        }

        if (burnTimer > 0)
        {
            burnTimer -= GameManager.GetDeltaTime();
            TakeDamage(GameManager.GetDeltaTime(), DamageType.Fire);

            tintColor.g = 0;
            tintColor.b = 0;
        }

        if (freezeTimer > 0)
        {
            tintColor.r = 0;
            freezeTimer -= GameManager.GetDeltaTime();
        }
        else
        {
            Think();
        }
        if (currentTint != tintColor)
        {
            currentTint = tintColor;
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = currentTint;
            }
        }

        // Do death all at the same time
        if (isDead)
        {
            Die();
        }
    }

    public virtual void CopyState(EnemyBase otherEnemy)
    {
        freezeTimer = otherEnemy.freezeTimer;
        burnTimer = otherEnemy.burnTimer;
    }

    public virtual void Freeze(float freezeDuration)
    {
        if (freezeDuration > 0)
        {
            freezeTimer = Mathf.Max(freezeTimer, freezeDuration);
            burnTimer = 0;
        }
        else
        {
            freezeTimer = freezeDuration;
        }
    }

    public virtual void Blow(Vector2 windDirection, float windDuration)
    {
        this.windDirection = windDirection;
        this.windDuration = windDuration;
    }

    public virtual void Burn(float burnDuration)
    {
        burnTimer = Mathf.Max(burnTimer, burnDuration);
    }

    public virtual void TakeDamage(float damageAmount, DamageType damageType = DamageType.None)
    {

        // Damage types
        if (damageType == DamageType.Fire) // Fire unfreezes
        {
            if (freezeTimer > 0) health -= MELT_DAMAGE;
            freezeTimer = 0;
        }

        health -= damageAmount;
        if (health <= 0)
        {
            killingType = damageType;
            isDead = true;
        }

    }

    public virtual Vector2 GetPosition()
    {
        return transform.position;
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
            Logger.Log($"chance: {chance} > weight: {weight} | dropped: {dropChance}", LogLevel.debug);
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
        if (DebugManager.GetConsoleVar("DrawEnemyPos") == 1)
        { 
            // TODO: Fix this weird workaround
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                new Vector3(transform.position.x, -transform.position.y, 0));
            EditorGUI.DrawRect(new Rect(screenPos.x -5, screenPos.y - 5,
                10, 10), POS_COLOR);
        }
    }
}
