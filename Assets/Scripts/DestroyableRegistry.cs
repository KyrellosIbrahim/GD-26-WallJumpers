using System.Collections.Generic;
using UnityEngine;

public class DestroyableRegistry : MonoBehaviour
{
    public static DestroyableRegistry Instance;
    public static List<GameObject> items = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public static void Register(GameObject obj)
    {
        items.Add(obj);
    }

    public static void Unregister(GameObject obj)
    {
        items.Remove(obj);
    }
}