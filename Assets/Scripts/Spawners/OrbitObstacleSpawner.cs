using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitObstacleSpawner : IObstacleSpawner
{
    private List<Satellite> _spawnedSatellites = new List<Satellite>();
        
    public OrbitObstacleSpawner(PlanetData planetData, int obstacleCount, int collectableCount)
    {
        int totalCount = obstacleCount + collectableCount;

        for (int x = 0; x < totalCount; ++x)
        {
            var satellite = ObjectPoolManager.Instance.SatelliteObjectPool.Get();

            float angle = (float)x / totalCount * 2.0f * Mathf.PI;

            satellite.Controller.transform.rotation = Quaternion.Euler(new Vector3(0f, angle));
            Vector2 localPosition = new Vector2(planetData.OrbitRadius * Mathf.Cos(angle), planetData.OrbitRadius * Mathf.Sin(angle));
            satellite.Controller.ParentAndSetPosition(localPosition, planetData.PlanetController.OrbitTransform);
            
            _spawnedSatellites.Add(satellite);
        }
    }

    public void RemoveSpawner()
    {
        while (_spawnedSatellites.Any())
        {
            ObjectPoolManager.Instance.SatelliteObjectPool.Return(_spawnedSatellites[0]);
            _spawnedSatellites.RemoveAt(0);
        }
    }
}
