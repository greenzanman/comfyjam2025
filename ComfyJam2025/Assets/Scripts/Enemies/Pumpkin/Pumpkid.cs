using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Pumpkidd : EnemyBase
{
    [SerializeField] private float groupRadius = 2f;
    [SerializeField] private float cohesionStrength = 0.3f; // how much they pull toward the group center
    [SerializeField] private float separationStrength = 0.2f; // mild push away from others
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private List<Sprite> sprites;

    private SpriteRenderer spriteRenderer;
    private float spriteTimer;
    protected override void InitializeEnemy()
    {
        health = maxHealth;

        spriteRenderer = transform.Find("Visual").GetComponent<SpriteRenderer>();
    }
    protected override void Think()
    {
        CenterStation target = GameManager.centerStation;

        if (target != null)
        {

            if (utils.FlatSqrDistance(GetPosition(), target.transform.position) <= contactRange * contactRange)
            {
                DealPlayerDamage(contactDamage);
                return;
            }

            spriteTimer += GameManager.GetDeltaTime();

            Vector2 moveDir = (target.transform.position - transform.position).normalized;

                // Find nearby other Pumpkidds
                var allPumpkidds = FindObjectsOfType<Pumpkidd>();
                var neighbors = allPumpkidds
                    .Where(p => p != this && Vector2.Distance(p.transform.position, transform.position) < groupRadius)
                    .ToList();

                if (neighbors.Count > 0)
                {
                    // Move slightly toward center of neighbors (cohesion)
                    Vector2 center = Vector2.zero;
                    foreach (var n in neighbors) center += (Vector2)n.transform.position;
                    center /= neighbors.Count;
                    Vector2 cohesionDir = (center - (Vector2)transform.position).normalized;

                    // Separation
                    Vector2 separationDir = Vector2.zero;
                    foreach (var n in neighbors)
                    {
                        Vector2 diff = (Vector2)transform.position - (Vector2)n.transform.position;
                        float dist = diff.magnitude;
                        separationDir += diff.normalized / Mathf.Max(dist, 0.1f);
                    }
                    separationDir.Normalize();

                    // Combine
                    moveDir = (moveDir + cohesionStrength * cohesionDir + separationStrength * separationDir).normalized;
                }

                Vector3 newPos = (Vector2)transform.position + moveDir * moveSpeed * GameManager.GetDeltaTime();
                transform.position = newPos;
        
                spriteRenderer.sprite = sprites[(int)(spriteTimer * 5) % 3];
            spriteRenderer.flipX = moveDir.x > 0;
        }

    }
}
