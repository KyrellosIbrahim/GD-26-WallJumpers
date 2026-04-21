using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            gameManager.TriggerLose();
        }
    }
}