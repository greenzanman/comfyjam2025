using System.Collections.Generic;
using UnityEngine;

public class SunbeamSpellDamaging : MonoBehaviour
{
    private CircleCollider2D beamCollider;
    [SerializeField] private float lifetime = 3;
    [SerializeField] private float moveSpeed = 20;
    private float age = 0;
    [SerializeField] private float damagePerSecond = 10;
    // Start is called before the first frame update
    void Start()
    {
        beamCollider = GetComponentInChildren<CircleCollider2D>();
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

        // Follow mouse
        transform.position = Vector3.MoveTowards(transform.position,
            GameManager.GetMousePos(), moveSpeed * GameManager.GetDeltaTime());

        // Locate all enemies within the bounds
        foreach (EnemyBase enemy in EnemyManager.GetEnemies())
        {
            if (beamCollider.OverlapPoint(enemy.GetPosition()))
            {
                enemy.TakeDamage(damagePerSecond * GameManager.GetDeltaTime(), DamageType.Disintegrate);
            }
        }
    }
}
