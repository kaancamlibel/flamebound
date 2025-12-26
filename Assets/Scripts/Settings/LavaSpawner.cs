using UnityEngine;

public class LavaSpawner : MonoBehaviour
{
    public Vector3 lavaOffSet = new Vector3(0, 3, 0);
    public float speed = 1f;
    private Vector3 upPosition;
    private Vector3 downPosition;
    private Vector3 targetPosition;

    void Start()
    {
        downPosition = transform.position;
        upPosition = transform.position + lavaOffSet;
        targetPosition = upPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (targetPosition == upPosition)
                targetPosition = downPosition;
            else
                targetPosition = upPosition;
        }
    }
}