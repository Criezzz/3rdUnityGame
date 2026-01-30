using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject groundSlimePrefab;
    public GameObject flyingSlimePrefab;
    public GameObject carrotPickupPrefab;

    [Header("Carrot Pickup Spawning")]
    public float carrotSpawnInterval = 5f;  // Reduced from 8 to 5 seconds
    public bool autoSpawnCarrots = true;

    [Header("Supply Drop Spawning")]
    public GameObject supplyDropPrefab;
    public float supplyDropSpawnInterval = 15f;
    public bool autoSpawnSupplyDrops = true;

    [Header("Enemy Spawning")]
    public bool autoSpawnEnemies = true;
    public float enemySpawnInterval = 4f;           // Base spawn interval
    public float minSpawnInterval = 1.5f;           // Minimum spawn interval (max difficulty)
    public float difficultyIncreaseRate = 0.1f;     // How much faster spawns get over time
    public int maxEnemiesOnScreen = 8;              // Limit total enemies
    
    private float currentSpawnInterval;
    private float gameTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentSpawnInterval = enemySpawnInterval;
        
        if (autoSpawnCarrots)
        {
            StartCoroutine(SpawnCarrotRoutine());
        }
        
        if (autoSpawnEnemies)
        {
            StartCoroutine(SpawnEnemyRoutine());
        }

        if (autoSpawnSupplyDrops)
        {
            StartCoroutine(SpawnSupplyDropRoutine());
        }
    }

    public void SpawnGroundSlime()
    {
        if (groundSlimePrefab == null) return;
        
        // Original spawn points - patrolScript handles the jump from here
        Vector3[] spawnPoints = new Vector3[] {
            new Vector3(0.558f, -2.634f, 0),
            new Vector3(-0.548f, -2.634f, 0)
        };
        Vector3 position = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(groundSlimePrefab, position, Quaternion.identity);
    }

    public void SpawnFlyingSlime()
    {
        if (flyingSlimePrefab == null) return;
        
        Vector3 position = new Vector3(Random.Range(-7.4f, 7.4f), 5.27f, 0);
        Instantiate(flyingSlimePrefab, position, Quaternion.identity);
    }

    public void SpawnCarrotPickup()
    {
        if (carrotPickupPrefab == null) return;

        // Spawn from top of screen at random x position
        Vector3 position = new Vector3(Random.Range(-6f, 6f), 5f, 0);
        Instantiate(carrotPickupPrefab, position, Quaternion.identity);
        Debug.Log("Carrot pickup spawned!");
    }

    private IEnumerator SpawnCarrotRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(carrotSpawnInterval);
            SpawnCarrotPickup();
        }
    }

    public void SpawnSupplyDrop()
    {
        if (supplyDropPrefab == null) return;

        // Spawn from top of screen at random x position
        Vector3 position = new Vector3(Random.Range(-6f, 6f), 6f, 0);
        Instantiate(supplyDropPrefab, position, Quaternion.identity);
        Debug.Log("Supply drop spawned!");
    }

    private IEnumerator SpawnSupplyDropRoutine()
    {
        // Initial delay before first supply drop
        yield return new WaitForSeconds(supplyDropSpawnInterval);
        
        while (true)
        {
            SpawnSupplyDrop();
            yield return new WaitForSeconds(supplyDropSpawnInterval);
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        // Initial delay before enemies start spawning
        yield return new WaitForSeconds(2f);
        
        while (true)
        {
            // Count current enemies
            int currentEnemyCount = CountEnemiesOnScreen();
            
            if (currentEnemyCount < maxEnemiesOnScreen)
            {
                SpawnRandomEnemy();
            }
            
            // Wait for spawn interval
            yield return new WaitForSeconds(currentSpawnInterval);
            
            // Gradually increase difficulty (decrease spawn interval)
            currentSpawnInterval = Mathf.Max(minSpawnInterval, 
                currentSpawnInterval - difficultyIncreaseRate * Time.deltaTime * 10f);
        }
    }

    private void SpawnRandomEnemy()
    {
        // 60% ground slime, 40% flying slime
        if (Random.value < 0.6f)
        {
            SpawnGroundSlime();
        }
        else
        {
            SpawnFlyingSlime();
        }
    }

    private int CountEnemiesOnScreen()
    {
        // Count all GameObjects with "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
    }
}
