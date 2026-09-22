using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance 
    {
        get 
        {
            if (instance == null) instance = FindAnyObjectByType<T>();
            return instance;
        }
    }
}