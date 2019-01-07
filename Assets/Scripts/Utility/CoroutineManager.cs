using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;
    public static CoroutineManager Instance
    {
        get
        {
            return _instance;
        }
    }
    
    private void Awake()
    {
        // Wont need this just for clarification.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // If the singleton hasn't been initialized yet
            _instance = this;
        }
    }
    
    public Coroutine StartIndependentCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }
    
    public void StopIndependentCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}
