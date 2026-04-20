using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float horizontalSmoothSpeed = 5f;
    public float verticalSmoothSpeed = 5f;
    public GameManager gameManager;

    private float highestY;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        highestY = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        float targetX = player.position.x;

        // Only update highestY if player has climbed above current camera Y
        if (player.position.y > highestY)
            highestY = player.position.y;

        float targetY = highestY;

        float newX = Mathf.Lerp(transform.position.x, targetX, horizontalSmoothSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, targetY, verticalSmoothSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);

        CheckLoseBoundary();
    }

    void CheckLoseBoundary()
    {
        float camBottomEdge = transform.position.y - cam.orthographicSize;

        if (player.position.y < camBottomEdge)
        {
            gameManager.TriggerLose();
        }
    }
    public void PurgeBelowCamera()
    {
        float camBottomEdge = transform.position.y - cam.orthographicSize;

        for (int i = DestroyableRegistry.items.Count - 1; i >= 0; i--)
        {
            GameObject obj = DestroyableRegistry.items[i];
            if (obj != null && obj.transform.position.y < camBottomEdge)
            {
                Destroy(obj);
            }
        }
    }
}