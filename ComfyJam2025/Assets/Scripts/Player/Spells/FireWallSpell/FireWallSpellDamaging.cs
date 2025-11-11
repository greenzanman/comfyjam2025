using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWallSpellDamaging : MonoBehaviour
{
    private BoxCollider2D wallCollider;
    [SerializeField] private float lifetime = 20;
    private float age = 0;
    [SerializeField] private float damagePerSecond = 1;
    // Start is called before the first frame update
    void Start()
    {
        wallCollider = GetComponentInChildren<BoxCollider2D>();
    }

    // Update is called once per frame
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
                enemy.TakeDamage(damagePerSecond * GameManager.GetDeltaTime(), DamageType.Fire);
            }
        }
    }
}
