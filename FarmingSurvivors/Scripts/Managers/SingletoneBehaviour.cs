using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SingletoneBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance 
    {
        get 
        { 
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            if(instance != this)
                Destroy(gameObject);
            
            return;
        }

        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }

}
