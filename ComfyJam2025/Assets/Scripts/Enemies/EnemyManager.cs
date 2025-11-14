using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    //[SerializeField] private Camera cam;
    public static EnemyManager instance;

    public List<GameObject> enemyPrefabs;

    // Simplistic 'spawn every x' enemies
    private float enemySpawnTimer = 0;
    private float waveTimerElapsedTime = 0;
    [SerializeField] private float enemySpawnInterval = 4f;

    [Header("TIMER")]
    [SerializeField] private float spawnTimePerWave = 20f;
    [SerializeField] private float waveDowntime = 5f;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI waveStateText;
    [SerializeField] private ParticleSystem enemyWavePS;

    [Header("DEBUG")]
    public bool canSpawn = true;

    public enum WaveState {
        SPAWNING_ENEMIES,
        DOWNTIME
    }

    private WaveState waveState;

    private static HashSet<EnemyBase> enemies = new HashSet<EnemyBase>();
    void Awake()
    {
        if (instance == null)
            instance = this;

        //Logger.Log("EnemyManager registered", LogLevel.info);
    }

    private void Start()
    {
        // Register some console variables
		DebugManager.RegisterConsoleVar("DrawEnemyHealth", 1);
		DebugManager.RegisterConsoleVar("DrawEnemyPos", 1);
    }

    private void Update()
    {
        if (!canSpawn) return;

        switch (waveState)
        {
            case WaveState.SPAWNING_ENEMIES:
                //enemyWavePS.Play();
                HandleEnemySpawning();
                break;
            case WaveState.DOWNTIME:
                //enemyWavePS.Stop();
                HandleWaveDownTime();
                break;
        }
        DisplayWaveTimer();

        // Hacks
#if UNITY_EDITOR
        EnemyHacks();
#endif
    }

    private void EnemyHacks()
    {
        // Spawn Gravy
        if (Input.GetKeyDown(KeyCode.G))
        {        
            Instantiate(enemyPrefabs[1], GameManager.GetMousePos(), Quaternion.identity);
        }
        // Spawn boombawk
        if (Input.GetKeyDown(KeyCode.F))
        {        
            Instantiate(enemyPrefabs[2], GameManager.GetMousePos(), Quaternion.identity);
        }
    }


    private void DisplayWaveTimer() {
        float upperLimitTime = 0f;
        waveTimerText.text = 0f.ToString("0:00");

        if (waveState == WaveState.SPAWNING_ENEMIES) {
            upperLimitTime = spawnTimePerWave;
            waveStateText.text = "protect mama!";
        }
        else if (waveState == WaveState.DOWNTIME) {
            upperLimitTime = waveDowntime;
            waveStateText.text = "gather your spells.";
        }

        if (upperLimitTime - waveTimerElapsedTime <= 10f) {
            waveTimerText.color = Color.red;
        }
        else {
            waveTimerText.color = Color.green;
        }
        upperLimitTime += 0.35f;
        waveTimerText.text = (upperLimitTime - waveTimerElapsedTime).ToString("0 : 00");
    }
    private void HandleWaveDownTime() {
        float deltaTime = GameManager.GetDeltaTime();
        waveTimerElapsedTime += deltaTime;

        if (waveTimerElapsedTime >= waveDowntime) {
            waveTimerElapsedTime = 0f;
            waveState = WaveState.SPAWNING_ENEMIES;
        }
    }

    private void HandleEnemySpawning()
    {
        float deltaTime = GameManager.GetDeltaTime();

        enemySpawnTimer -= deltaTime;
        waveTimerElapsedTime += deltaTime;

        if (waveTimerElapsedTime >= spawnTimePerWave) {
            waveTimerElapsedTime = 0f;
            waveState = WaveState.DOWNTIME;
            return;
        }
        
        if (enemySpawnTimer < 0)
        {
            enemySpawnTimer += enemySpawnInterval;
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        // Generate random spot 0.5 unit within the border
        float aspect = Screen.width / Screen.height;
        float width = 4.0f * Camera.main.orthographicSize * aspect;
        float height = 2.0f * Camera.main.orthographicSize * 1.25f;
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
    public static EnemyBase GetClosestEnemyExcludingSelf(EnemyBase self, Vector3 position, float cutoffDistance = Mathf.Infinity) {
        EnemyBase result = null;
        float closest = cutoffDistance * cutoffDistance;
        // TODO: Reduce the calculations on this somehow, for large numbers
        foreach (EnemyBase enemy in enemies) {
            if (enemy == self) continue;
            float candDistance = (position - enemy.transform.position).sqrMagnitude;
            if (candDistance < closest) {
                closest = candDistance;
                result = enemy;
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
