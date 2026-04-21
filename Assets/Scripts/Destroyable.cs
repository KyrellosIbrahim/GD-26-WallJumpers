using UnityEngine;

public class Destroyable : MonoBehaviour
{
    void OnEnable()
    {
        DestroyableRegistry.Register(gameObject);
    }

    void OnDestroy()
    {
        DestroyableRegistry.Unregister(gameObject);
    }
}