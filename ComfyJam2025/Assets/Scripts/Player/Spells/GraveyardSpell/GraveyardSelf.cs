using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardSelf : MonoBehaviour
{
    public GameObject gremlinPrefab;
    private float age;
    [SerializeField] private float lifetime = 20;

    private float gremlinTimer = 0;
    private const float TARGET_RADIUS = 15;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        age += GameManager.GetDeltaTime();
        if (age > lifetime)
            Destroy(gameObject);

        gremlinTimer -= GameManager.GetDeltaTime();

        if (gremlinTimer < 0)
        {
            EnemyBase closestEnemy = EnemyManager.GetClosestEnemy(transform.position, TARGET_RADIUS);
            if (closestEnemy)
            {
                // Spawn dude
                gremlinTimer += 2f;

                Vector2 spawnPos = Random.insideUnitCircle * 2;
                GameObject gremlinObject = Instantiate(gremlinPrefab, transform.position + new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                Gremlin gremlin = gremlinObject.GetComponent<Gremlin>();
                gremlin.target = closestEnemy;
            }
            else
            {
                // Wait a bit
                gremlinTimer += 0.1f;
            }
        }
    }
}
