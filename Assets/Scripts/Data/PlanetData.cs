using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetData
{
    public PlanetData(GameObject planetObject, Vector2 positionVariation, bool isSmallPlanet)
    {
        PlanetObject = planetObject;
        PositionVariation = positionVariation;
        PlanetController = PlanetObject.GetComponent<PlanetController>();
        OrbitRadius = PlanetObject.GetComponentInChildren<CircleCollider2D>().radius;
        ObstacleSpawners = new List<IObstacleSpawner>();
        IsSmallPlanet = isSmallPlanet;
    }

    public GameObject PlanetObject { get; private set; }
    public bool IsSmallPlanet { get; private set; }
    public Vector2 PositionVariation { get; set; }
    public List<IObstacleSpawner> ObstacleSpawners { get; private set; }
    public PlanetController PlanetController { get; private set; }
    public float OrbitRadius { get; private set; }

    public void CleanUpObstacles()
    {
        while (ObstacleSpawners.Any())
        {
            ObstacleSpawners[0].RemoveSpawner();
            ObstacleSpawners[0] = null;
            ObstacleSpawners.RemoveAt(0);
        }
    } 
}
