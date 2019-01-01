using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstaclePoolItem : ObjectPoolItem
{
    public enum ObstacleColour
    {
        Blue,
        Yellow
    }

    public ObstacleColour Colour;
}
