using UnityEngine;
using Pathfinding;

public class flyingScript : MonoBehaviour
{
    public GameObject player;
    void Start() 
    {
        player = GameObject.Find("player");

        gameObject.GetComponent<AIDestinationSetter>().target = player.transform;
    }

    void Update()
    {
    
    }
}