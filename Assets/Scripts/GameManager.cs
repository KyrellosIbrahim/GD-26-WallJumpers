using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void TriggerLose()
    {
        Debug.Log("Player fell off screen — game over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}