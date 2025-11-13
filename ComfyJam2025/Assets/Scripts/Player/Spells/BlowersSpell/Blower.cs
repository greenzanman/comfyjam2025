using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blower : MonoBehaviour
{
    float moveSpeed = 6;
    float patience = 0.5f;
    public EnemyBase target;
    public Vector2 basePos;
    public Vector2 windDirection;
    float age = 0;
    SpriteRenderer spriteRenderer;
    [SerializeField] private float targetRadius = 6;
    [SerializeField] private float windStrength = 4f;


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    void Update()
    {
        // Bouncing
        age += GameManager.GetDeltaTime() * 4;
        spriteRenderer.transform.localPosition = new Vector2(0, 0.5f * Mathf.Abs(Mathf.Cos(age)));
        spriteRenderer.transform.rotation = Quaternion.Euler(Vector3.forward * Mathf.Cos(age) * 20);

        if (target == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, basePos, moveSpeed * GameManager.GetDeltaTime());

            // Try a few times to get a new target
            patience -= GameManager.GetDeltaTime();
            if (patience < 0)
            {
                target = EnemyManager.GetClosestEnemy(basePos, targetRadius);
                patience += 0.25f;
            }

        }
        else
        {
            if (utils.FlatSqrDistance(basePos, target.GetPosition()) > targetRadius * targetRadius)
            {
                target = null;
                patience = 0f;
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, target.GetPosition(), moveSpeed * GameManager.GetDeltaTime());

            if (utils.FlatSqrDistance(transform.position, target.GetPosition()) < 1)
            {
                // TODO: Play animation
                target.Blow(windDirection, windStrength);
                patience = 1f;
                target = null;
            }
        }
    }
}
