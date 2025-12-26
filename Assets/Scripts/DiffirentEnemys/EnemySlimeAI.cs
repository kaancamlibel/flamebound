using UnityEngine;

public class EnemySlimeAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public GameObject[] wayPoints;

    int nextWayPoint = 0;
    float distToPoint;

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (wayPoints == null || wayPoints.Length == 0) return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[nextWayPoint].transform.position, moveSpeed * Time.deltaTime);
        distToPoint = Vector2.Distance(transform.position, wayPoints[nextWayPoint].transform.position);

        if (distToPoint < 0.2f)
        {
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        Vector3 currRot = transform.eulerAngles;
        currRot.z = wayPoints[nextWayPoint].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
        ChooseNextWayPoint();
    }

    void ChooseNextWayPoint()
    {
        nextWayPoint++;
        if (nextWayPoint >= wayPoints.Length)
        {
            nextWayPoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (wayPoints == null) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < wayPoints.Length; i++)
        {
            if (wayPoints[i] == null) continue;

            Gizmos.DrawSphere(wayPoints[i].transform.position, 0.15f);

            if (i + 1 < wayPoints.Length)
            {
                Gizmos.DrawLine(
                    wayPoints[i].transform.position,
                    wayPoints[i + 1].transform.position
                );
            }
        }
    }
}
