using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AsteroidData : ObstaclePoolItem
{
    public AsteroidController AsteroidController;
    
    protected override void PostInitialise()
    {
        AsteroidController = Object.GetComponent<AsteroidController>();
    }
}
