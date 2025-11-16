using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SunbeamSpellDamaging : MonoBehaviour
{
    private CircleCollider2D beamCollider;
    [SerializeField] private float lifetime = 3;
    [SerializeField] private float moveSpeed = 20;
    private float age = 0;
    [SerializeField] private float damagePerSecond = 10;

    [SerializeField] private VisualEffect spellVFX;
    [SerializeField] private VisualEffect secondaryVFX;
    private const string VFX_EVENT_NAME = "OnAbilityCasted";
    private float vfxDelay = 0.1f;
    private float currentDelay = 0f;
    private float spellLocationOffset = 3f;


    void Start()
    {
        beamCollider = GetComponentInChildren<CircleCollider2D>();
    }

    void Update()
    {
        // Destroy after a time
        age += GameManager.GetDeltaTime();
        if (age > lifetime)
        {
            Destroy(gameObject);
        }
        currentDelay += Time.deltaTime;

        // Follow mouse
        transform.position = Vector3.MoveTowards(transform.position,
            GameManager.GetMousePos(), moveSpeed * GameManager.GetDeltaTime());

        // Locate all enemies within the bounds
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (beamCollider.OverlapPoint(enemy.GetPosition()))
            {
                if (currentDelay >= vfxDelay) {
                    PlayVFX(enemy.transform.position, enemy.transform);
                    currentDelay = 0f;
                }
                
                enemy.TakeDamage(damagePerSecond * GameManager.GetDeltaTime(), DamageType.Disintegrate);
            }
        }
    }
    protected virtual void PlayVFX(Vector3 targetPosition, Transform secondaryTransformTarget = null) {
        if (spellVFX) {
            VisualEffect vfx = Instantiate(spellVFX);
            vfx.transform.position = new Vector3(targetPosition.x, targetPosition.y + (-vfx.GetVector3("Direction").y / spellLocationOffset), 0f); ;
            vfx.SendEvent(VFX_EVENT_NAME);
        }
        if (secondaryVFX) { 
            VisualEffect vfx2 = Instantiate(secondaryVFX, secondaryTransformTarget);
            vfx2.transform.position = new Vector3(secondaryTransformTarget.transform.position.x, secondaryTransformTarget.transform.position.y + (-vfx2.GetVector3("Direction").y / spellLocationOffset), 0f); ;
            vfx2.SendEvent(VFX_EVENT_NAME);
        }
    }
}
