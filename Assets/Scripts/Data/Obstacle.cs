using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Obstacle<T> : PoolObject where T: ObstacleController
{
    public T Controller;
    private ObstacleColour _colour;

    public ObstacleColour Colour
    {
        get { return _colour; }
        set
        {
            _colour = value;
            Controller.SetColour(_colour);
        }
    }
    
    protected override void PostInitialise()
    {
        Controller = Object.GetComponent<T>();
    }

    public override void FetchedFromPool()
    {
        base.FetchedFromPool();
        var randomColourIndex = Random.Range(0, 2);
        Colour = (ObstacleColour)randomColourIndex;
        Controller.RevealObstacle();
    }

    public override void ReturnedToPool()
    {
        base.ReturnedToPool();
        Controller.HideObstacle();
    }
}

public enum ObstacleColour
{
    Blue,
    Yellow
}