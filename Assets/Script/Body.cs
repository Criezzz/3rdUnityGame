using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public PlayerController player;
    void Awake()
    {
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
         player.rg.linearVelocity = new Vector2(0, player.rg.linearVelocity.y);
    }
}
