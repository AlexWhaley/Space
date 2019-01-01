using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarData
{
    public StarData(GameObject starObject, Vector2 positionVariation)
    {
        StarObject = starObject;
        PositionVariation = positionVariation;
    }

    public GameObject StarObject { get; private set; }
    public Vector2 PositionVariation { get; set; }

}
