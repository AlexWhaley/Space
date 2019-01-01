using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetData
{
    public PlanetData(GameObject planetObject, Vector2 positionVariation)
    {
        PlanetObject = planetObject;
        PositionVariation = positionVariation;
        PlanetController = PlanetObject.GetComponent<PlanetController>();
    }

    public GameObject PlanetObject { get; private set; }
    public Vector2 PositionVariation { get; set; }
    public List<GameObject> ConnectingObstacles { get; set; }
    public PlanetController PlanetController { get; private set; }
}
