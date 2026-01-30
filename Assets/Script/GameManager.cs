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
    public float carrotSpawnInterval = 8f;
    public bool autoSpawnCarrots = true;

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
        if (autoSpawnCarrots)
        {
            StartCoroutine(SpawnCarrotRoutine());
        }
    }

    public void SpawnGroundSlime()
    {
        Vector3[] spawnPoints = new Vector3[] {
            new Vector3(0.558f, -2.634f, 0),
            new Vector3(-0.548f, -2.634f, 0)
        };
        Vector3 position = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(groundSlimePrefab, position, Quaternion.identity);
    }

    public void SpawnFlyingSlime()
    {
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

    void Update()
    {
        
    }
}
