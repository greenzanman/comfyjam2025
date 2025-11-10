using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnEntry
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float spawnProbability;
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("Enemy Spawn Settings")]
    public List<EnemySpawnEntry> enemyPrefabs = new List<EnemySpawnEntry>();

    // Simplistic 'spawn every x' enemies
    private float enemySpawnTimer = 0;
    [SerializeField] private float ENEMY_SPAWN_RATE = 2f;

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
		DebugManager.RegisterConsoleVar("DrawEnemyHealth", 1);
		DebugManager.RegisterConsoleVar("DrawEnemyPos", 1);
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
        // Choose an enemy prefab based on weighted probabilities
        GameObject chosenPrefab = GetWeightedRandomEnemy();

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
        Instantiate(chosenPrefab, spawnPos, Quaternion.identity);
    }

    private GameObject GetWeightedRandomEnemy()
    {
        float totalWeight = enemyPrefabs.Sum(e => Mathf.Max(e.spawnProbability, 0f));
        if (totalWeight <= 0f)
            return enemyPrefabs[0].prefab; // fallback

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var entry in enemyPrefabs)
        {
            cumulative += Mathf.Max(entry.spawnProbability, 0f);
            if (roll <= cumulative)
                return entry.prefab;
        }

        return enemyPrefabs.Last().prefab; // fallback
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
        if (enemies == null || enemies.Count == 0)
            return null;

        EnemyBase result = null;
        float closest = cutoffDistance * cutoffDistance;
        // TODO: Reduce the calculations on this somehow, for large numbers
        foreach (var enemy in enemies.ToArray())
        {
            if (enemy == null) // destroyed object
            {
                enemies.Remove(enemy);
                continue;
            }

            try
            {
            float candDistance = (position - enemy.transform.position).sqrMagnitude;
            if (candDistance < closest)
            {
                closest = candDistance;
                result = enemy;
            }
            }
            catch (MissingReferenceException)
            {
                enemies.Remove(enemy);
            }

        }
        return result;
    }

    // TODO: Move this into being delegate based
    public static List<EnemyBase> GetEnemies()
    {
        return enemies.ToList();
    }
}
