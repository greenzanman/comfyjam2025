using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<GameObject> enemyPrefabs;

    // Simplistic 'spawn every x' enemies
    private float enemySpawnTimer = 0;
    private float ENEMY_SPAWN_RATE = 4f;

    private static HashSet<EnemyBase> enemies = new HashSet<EnemyBase>();
    void Awake()
    {
        if (instance == null)
            instance = this;

        Logger.Log("EnemyManager registered", LogLevel.info);
    }

    private void Start()
    {
        // Register some console variables
		DebugManager.RegisterConsoleVar("DrawEnemyHealth", 0);
    }

    private void Update()
    {
        HandleEnemySpawning();
    }

    private void HandleEnemySpawning()
    {
        float deltaTime = GameManager.GetDeltaTime();

        enemySpawnTimer -= deltaTime;
        if (enemySpawnTimer < 0)
        {
            enemySpawnTimer += ENEMY_SPAWN_RATE;
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        // Generate random spot 0.5 unit within the border
        float width = 2 * GameplayStatics.SCREEN_WIDTH - 1;
        float height = 2 * GameplayStatics.SCREEN_HEIGHT - 1;
        float randVal = Random.value * 2 * (width + height);

        Vector3 spawnPos;

        if (randVal < 2 * width)
        {
            spawnPos = new Vector2(randVal % width - width / 2,
                (Mathf.Floor(randVal / width) * 2 - 1) * height / 2);
        }
        else
        {
            randVal -= 2 * width;

            spawnPos = new Vector2((Mathf.Floor(randVal / height) * 2 - 1) * width / 2,
                randVal % height - height / 2);
        }
        DebugManager.DisplayDebug(spawnPos.ToString());
        Instantiate(enemyPrefabs[0], spawnPos, Quaternion.identity);
    }

    public static void RegisterEnemy(EnemyBase newEnemy)
    {
        enemies.Add(newEnemy);
    }

    public static void DeregisterEnemy(EnemyBase oldEnemy)
    {
        if (!enemies.Remove(oldEnemy))
        {
            Logger.Log($"Attempted to remove unregistered enemy: {oldEnemy.name}", LogLevel.error);
        }
    }

    public static EnemyBase GetClosestEnemy(Vector3 position, float cutoffDistance = Mathf.Infinity)
    {
        EnemyBase result = null;
        float closest = cutoffDistance * cutoffDistance;
        // TODO: Reduce the calculations on this somehow, for large numbers
        foreach (EnemyBase enemy in enemies)
        {
            float candDistance = (position - enemy.transform.position).sqrMagnitude;
            if (candDistance < closest)
            {
                closest = candDistance;
                result = enemy;
            }
        }
        return result;
    }

    // TODO: Move this into being delegate based
    public static HashSet<EnemyBase> GetEnemies()
    {
        return enemies;
    }
}
