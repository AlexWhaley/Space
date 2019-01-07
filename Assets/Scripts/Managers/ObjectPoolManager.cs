using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
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

    [SerializeField]
    private AsteroidObjectPool _asteroidPool;

    public AsteroidObjectPool AsteroidPool
    {
        get { return _asteroidPool; }
    }
}

[System.Serializable]
public class AsteroidObjectPool : ObjectPool<Asteroid>
{
}
