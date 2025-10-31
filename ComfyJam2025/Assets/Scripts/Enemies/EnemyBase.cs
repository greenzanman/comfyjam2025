using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float health { get; protected set; }

    protected float maxHealth = 4;

    private CenterStation centerStation;

    private Color HEALTH_COLOR = Color.red;

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
    }
}
