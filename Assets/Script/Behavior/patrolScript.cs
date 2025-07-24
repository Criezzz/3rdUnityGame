using UnityEngine;
using System.Collections;

public class patrolScript : MonoBehaviour
{
    public float popHeight = 2f;
    public float popDuration = 0.5f;
    public float patrolSpeed = 2f;
    public float waitAtEdge = 0.5f;

    private Vector3 landPoint;
    private Vector3 leftEdge;
    private Vector3 rightEdge;
    private Vector3 patrolTargetEdge;
    private bool isPatrolling = false;
    private bool goingToEdge = true;
    private bool waiting = false;

    void Start()
{
    Vector3 spawnPoint = transform.position;
    float y = spawnPoint.y;
    float z = spawnPoint.z;

    if (spawnPoint.x < 0)
        landPoint = new Vector3(-2.65f, y, z);
    else
        landPoint = new Vector3(2.77765f, y, z);

    leftEdge = new Vector3(-7.570161f, y, z);
    rightEdge = new Vector3(7.582f, y, z);

    if (landPoint.x < 0)
        patrolTargetEdge = leftEdge;
    else
        patrolTargetEdge = rightEdge;

    StartCoroutine(PopAndLand(spawnPoint, landPoint));
}

IEnumerator PopAndLand(Vector3 start, Vector3 end)
{
    float timer = 0f;
    while (timer < popDuration)
    {
        float t = timer / popDuration;
        float yOffset = 4 * popHeight * t * (1 - t);
        transform.position = Vector3.Lerp(start, end, t) + new Vector3(0, yOffset, 0);
        timer += Time.deltaTime;
        yield return null;
    }
    transform.position = end;
    isPatrolling = true;
}

    void Update()
    {
        if (isPatrolling && !waiting)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector3 target = goingToEdge ? patrolTargetEdge : landPoint;
        transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    IEnumerator WaitAndSwitch()
    {
        waiting = true;
        yield return new WaitForSeconds(waitAtEdge);
        goingToEdge = !goingToEdge;
        waiting = false;
    }
}
