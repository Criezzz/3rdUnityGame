using UnityEngine;

public class flyingScript : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool movingToTarget = true;

    void Start()
    {
        Vector3 startPos = transform.position;
        targetPosition = new Vector3(startPos.x, 4f, startPos.z);
    }

    void Update()
    {
        if (movingToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition; 
                movingToTarget = false; 
        }
    }
    }
}