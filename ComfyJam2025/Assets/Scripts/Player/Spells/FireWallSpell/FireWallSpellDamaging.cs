using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireWallSpellDamaging : MonoBehaviour
{
    private BoxCollider2D wallCollider;
    [SerializeField] private float lifetime = 20;
    private float age = 0;
    [SerializeField] private float damagePerSecond = 1;

    [SerializeField] private VisualEffect spellVFX;
    private const string VFX_EVENT_NAME = "OnAbilityCasted";

    void Start()
    {
        wallCollider = GetComponentInChildren<BoxCollider2D>();
    }

    void Update()
    {
        // Destroy after a time
        age += GameManager.GetDeltaTime();
        if (age > lifetime)
        {
            Destroy(gameObject);
        }
        // Locate all enemies within the bounds
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (wallCollider.OverlapPoint(enemy.GetPosition()))
            {
                PlayVFX(enemy.transform.position, enemy.transform);
                enemy.TakeDamage(damagePerSecond * GameManager.GetDeltaTime(), DamageType.Fire);
            }
        }
    }
    protected virtual void PlayVFX(Vector3 targetPosition, Transform target) {
        if (spellVFX) {
            VisualEffect vfx = Instantiate(spellVFX, target);
            vfx.transform.position = new Vector3(targetPosition.x, targetPosition.y + (-vfx.GetVector3("Direction").y / 3f), 0f); ;
            vfx.SendEvent(VFX_EVENT_NAME);
        }
    }
}
