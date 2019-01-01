using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool<T> where T : ObjectPoolItem, new()
{
    [SerializeField]
    private GameObject _objectPrefab;
    [SerializeField]
    private Transform _poolParent;
    [SerializeField]
    private int _quanitityOfInstancesOnStart = 0;

    private List<T> _objectPool = new List<T>(16);

    public T Get()
    {
        T poolItem;
        if (_objectPool.Count > 0)
        {
            poolItem = _objectPool[0];
            _objectPool.RemoveAt(0);
        }
        else
        {
            poolItem = new T();
            poolItem.Initialise(_objectPrefab, _poolParent);
        }
        return poolItem;
    }

    public void Return(T poolItem)
    {
        _objectPool.Add(poolItem);
    }
}
