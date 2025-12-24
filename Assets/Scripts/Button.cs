using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject buttonOff, buttonOn;
    public Transform wall;

    public Vector3 wallOpenOffset = new Vector3(0, 5, 0);
    public float moveSpeed = 3f;

    private Vector3 closedPos;
    private Vector3 openedPos;
    private Vector3 targetPos;

    void Start()
    {
        closedPos = wall.position; 
        openedPos = closedPos + wallOpenOffset; 
        targetPos = closedPos;
    }

    void Update()
    {
        wall.position = Vector3.MoveTowards(wall.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LightKey"))
        {
            buttonOff.SetActive(false);
            buttonOn.SetActive(true);
            targetPos = openedPos;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LightKey"))
        {
            buttonOff.SetActive(true);
            buttonOn.SetActive(false);
            targetPos = closedPos;
        }
    }
}