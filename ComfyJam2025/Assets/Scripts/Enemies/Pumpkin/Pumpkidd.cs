using UnityEngine;
using System.Linq;

public class Pumpkidd : PumpkinBase
{
    [SerializeField] private float groupRadius = 2f;
    [SerializeField] private float cohesionStrength = 0.3f; // how much they pull toward the group center
    [SerializeField] private float separationStrength = 0.2f; // mild push away from others

    protected override void MoveTowardTarget(Vector2 targetPos, float dt)
    {
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;

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

        Vector2 newPos = (Vector2)transform.position + moveDir * moveSpeed * dt;
        if (rb) rb.MovePosition(newPos);
        else transform.position = newPos;
    }

    protected override void AttackPlayer()
    {
        base.AttackPlayer();
        // Dies after attacking mama
        Die();
    }
}
