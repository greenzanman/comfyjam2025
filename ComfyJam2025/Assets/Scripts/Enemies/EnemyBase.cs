using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float health { get; protected set; }

    [SerializeField] protected float maxHealth = 4;

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

    protected virtual void Update()
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
        Logger.Log("BURNING! Burn called on " + name, LogLevel.debug);
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
        // Inform manager
        EnemyManager.DeregisterEnemy(this);

        Destroy(gameObject);
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
