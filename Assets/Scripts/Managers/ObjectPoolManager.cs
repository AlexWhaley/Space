using ObjectPools;
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
    
    [SerializeField]
    private AsteroidFragmentsObjectPool _asteroidFragmentsObjectPool;

    public AsteroidFragmentsObjectPool AsteroidFragmentsObjectPool
    {
        get { return _asteroidFragmentsObjectPool; }
    }
    
    [SerializeField]
    private SatelliteObjectPool _satelliteObjectPool;

    public SatelliteObjectPool SatelliteObjectPool
    {
        get { return _satelliteObjectPool; }
    }
}

namespace ObjectPools
{
    [System.Serializable]
    public class AsteroidObjectPool : ObjectPool<Asteroid>
    {
    }

    [System.Serializable]
    public class AsteroidFragmentsObjectPool : ObjectPool<AsteroidFragments>
    {
    }
    
    [System.Serializable]
    public class SatelliteObjectPool : ObjectPool<Satellite>
    {
    }
}


