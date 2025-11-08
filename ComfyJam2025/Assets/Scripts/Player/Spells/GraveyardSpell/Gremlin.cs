using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gremlin : MonoBehaviour
{
    float moveSpeed = 2;
    float patience = 0.5f;
    public EnemyBase target;
    float age = 0;
    SpriteRenderer spriteRenderer;
    [SerializeField] private float damage = 2;
    int tries = 5;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Bouncing
        age += GameManager.GetDeltaTime() * 4;
        spriteRenderer.transform.localPosition = new Vector2(0, 0.5f * Mathf.Abs(Mathf.Cos(age)));
        spriteRenderer.transform.rotation = Quaternion.Euler(Vector3.forward * Mathf.Cos(age) * 20);

        if (target == null)
        {
            // Try a few times to get a new target
            patience -= GameManager.GetDeltaTime();
            if (patience < 0)
            {
                target = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), 6);
                patience += 0.5f;
                tries -= 1;
                if (tries < 0)
                    Destroy(gameObject);

            }

        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target.GetPosition(), moveSpeed * GameManager.GetDeltaTime());

            if (utils.FlatSqrDistance(transform.position, target.GetPosition()) < 1)
            {
                // TODO: Play animation
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
