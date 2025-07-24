using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject groundSlimePrefab;
    public GameObject flyingSlimePrefab;

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
    // Update is called once per frame
    void Update()
    {
        
    }
}
