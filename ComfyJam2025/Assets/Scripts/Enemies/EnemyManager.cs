using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using System;

public class EnemyManager : MonoBehaviour
{
    //[SerializeField] private Camera cam;
    public static EnemyManager instance;

    [Serializable]
    public class Wave {
        
        [Serializable]
        public class EnemiesInWave {
            [field: SerializeField] public GameObject EnemyPrefab { get; set; }
            [field: SerializeField] public float SpawnChance { get; set; } = 0.5f;

        }

        [field: SerializeField] public GameObject InitialEnemyPrefab { get; set; }
        [field: SerializeField] public int InitialEnemyCount { get; set; } = 0;

        public List<EnemiesInWave> enemiesInWave;
        public int InitialEnemiesSpawned { get; set; } = 0;

        public EnemyBase SpawnEnemy(Vector3 spawnPosition, bool initial) {
            EnemyBase enemy = null;

            if (InitialEnemiesSpawned < InitialEnemyCount && initial && InitialEnemyPrefab != null) {
                enemy = Instantiate(InitialEnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyBase>();
                InitialEnemiesSpawned++;
            }
            else {
                float chance = UnityEngine.Random.Range(0, GetEnemyWeights());

                foreach (EnemiesInWave enemies in enemiesInWave) {
                    if (enemies.SpawnChance >= chance) {
                        return Instantiate(enemies.EnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyBase>();
                    }
                    chance -= enemies.SpawnChance;
                }
            }
            return enemy;
        }
        private float GetEnemyWeights() {
            float weight = 0;

            foreach (EnemiesInWave enemy in enemiesInWave) {
                weight += enemy.SpawnChance;
            }
            return weight;
        }
    }

    public List<Wave> waves;   
    [SerializeField] private float enemySpawnInterval = 4f;
    [SerializeField] private float spawnTimePerWave = 20f;
    [SerializeField] private float waveDowntime = 5f;
    [SerializeField] private VisualEffect finalDeathVFX;

    [Header("TIMER UI")]    
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI waveStateText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private ParticleSystem enemyWavePS;
    [SerializeField] private Color enemySpawningStateColor;
    [SerializeField] private Color downTimeStateColor;

    [Header("DEBUG")]
    public bool canSpawn = true;

    private float enemySpawnTimer = 0;
    private float waveTimerElapsedTime = 0;
    private int currentWave = 0;
    private bool currentWavesUpdated = false;

    public enum WaveState {
        SPAWNING_ENEMIES,
        DOWNTIME,
        GAME_END
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
        // Tell AudioManager to play battle music
        AudioManager.instance.PlayMusic(MusicTrack.IntenseBattle, 0.3f);
        // Register some console variables
		//DebugManager.RegisterConsoleVar("DrawEnemyHealth", 1);
		//DebugManager.RegisterConsoleVar("DrawEnemyPos", 1);
    }

    private void Update()
    {
        if (!canSpawn) return;

        switch (waveState)
        {
            case WaveState.SPAWNING_ENEMIES:
                enemyWavePS.Play();
                HandleEnemySpawning();
                currentWavesUpdated = false;
                break;
            case WaveState.DOWNTIME:
                enemyWavePS.Stop();
                HandleWaveDownTime();
                if (!currentWavesUpdated) {
                    currentWavesUpdated = true;
                    currentWave++;
                    if (currentWave >= waves.Count) {
                        waveState = WaveState.GAME_END;
                    }
                }
                break;
            case WaveState.GAME_END:
                // Tell AudioManager to play calm theme
                AudioManager.instance.PlayMusic(MusicTrack.CalmTheme);
                
                KillAllRemainingEnemies();
                break;
        }
        DisplayWaveStats();

        // Hacks
#if UNITY_EDITOR
        EnemyHacks();
#endif
    }

    private void EnemyHacks()
    {
        // // Spawn Gravy
        // if (Input.GetKeyDown(KeyCode.G))
        // {        
        //     Instantiate(waves[UnityEngine.Random.Range(0, waves.Count)].SpawnEnemy(Vector3.zero, false), GameManager.GetMousePos(), Quaternion.identity);
        // }
        // // Spawn boombawk
        // if (Input.GetKeyDown(KeyCode.F))
        // {        
        //     Instantiate(enemyPrefabs[2], GameManager.GetMousePos(), Quaternion.identity);
        // }
    }
    private void KillAllRemainingEnemies() {

        if (enemies.Count <= 0) return;

        foreach (EnemyBase enemy in enemies) {
            if (enemy == null) continue;
            Instantiate(finalDeathVFX, enemy.transform.position, Quaternion.identity).SendEvent("OnAbilityCasted");
            Destroy(enemy.gameObject);
        }
    }
    private void DisplayWaveStats() {

        if (waveState == WaveState.GAME_END) {
            waveStateText.text = "The End";
            waveTimerText.text = "";
            currentWaveText.text = "";
            waveStateText.color = Color.white;
            return;
        }

        float upperLimitTime = 0f;
        waveTimerText.text = 0f.ToString("0:00");

        if (waveState == WaveState.SPAWNING_ENEMIES) {
            upperLimitTime = spawnTimePerWave;
            waveStateText.color = enemySpawningStateColor;
            waveStateText.text = "protect mama!";
        }
        else if (waveState == WaveState.DOWNTIME) {
            upperLimitTime = waveDowntime;
            waveStateText.color = downTimeStateColor;
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
        
        if (waveState == WaveState.SPAWNING_ENEMIES) {
            currentWaveText.text = $"Wave: {currentWave+1}/{waves.Count}";
        }
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
        float randVal = UnityEngine.Random.value * 2 * (width + height);

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
        //DebugManager.DisplayDebug(spawnPos.ToString());
        waves[currentWave].SpawnEnemy(spawnPos, true);
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

    public static EnemyBase GetClosestEnemy(Vector3 position, float cutoffDistance = Mathf.Infinity, bool includeDeer = true)
    {
        EnemyBase result = null;
        float closest = cutoffDistance * cutoffDistance;
        // TODO: Reduce the calculations on this somehow, for large numbers
        foreach (EnemyBase enemy in enemies)
        {
            if (enemy == null) continue;
            // Skip over deer
            if (enemy is Deer && !includeDeer) continue;
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