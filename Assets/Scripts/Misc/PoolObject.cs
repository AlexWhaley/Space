using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObject
{
    private GameObject _object;

    public void Initialise(GameObject _objectPrefab, Transform _poolParent)
    {
        _object = GameObject.Instantiate(_objectPrefab, _poolParent);
        PostInitialise();
    }

    protected virtual void PostInitialise() { }

    public virtual void FetchedFromPool() { }

    public virtual void ReturnedToPool() { }

    protected GameObject Object
    {
        get
        {
            return _object;
        }
        private set { }
    }
}
