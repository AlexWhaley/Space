using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner
{
    private const float OffscreenOffset = 2.0f;
    private Vector2 _startPosition;
    private Vector2 _asteroidTrajectory;
    private bool _isLeftTrajectory;
    private float _waitInterval;
    private float _asteroidTravelTime;
    
    private List<Asteroid> SpawnedAsteroids = new List<Asteroid>();
    private Coroutine _asteroidSpawnRoutine;
    private Coroutine _asteroidDespawnRoutine;

    public AsteroidSpawner(Vector2 spawnerPosition, Vector2 planetPath, bool isLeftTrajectory, float asteroidSpeed, float asteroidsPerSecond)
    {
        // The effective equation of the line that bisects the line between the two planets
        Vector2 planetPathBisector = new Vector2(planetPath.y, -planetPath.x).normalized;
        
        float rightSpawnXDistanceOffset = CameraUtilities.CameraRightEdgeToWorld + OffscreenOffset - spawnerPosition.x;
        float rightSpawnYDistanceOffset = rightSpawnXDistanceOffset / planetPathBisector.x * planetPathBisector.y;
        Vector2 rightSideSpawnPosition = new Vector2(spawnerPosition.x + rightSpawnXDistanceOffset,spawnerPosition.y + rightSpawnYDistanceOffset);
        
        float leftSpawnXDistanceOffset = CameraUtilities.CameraLeftEdgeToWorld - OffscreenOffset - spawnerPosition.x;
        float leftSpawnYDistanceOffset = leftSpawnXDistanceOffset / planetPathBisector.x * planetPathBisector.y;
        Vector2 leftSideSpawnPosition = new Vector2(spawnerPosition.x + leftSpawnXDistanceOffset,spawnerPosition.y + leftSpawnYDistanceOffset);
        
        _asteroidTrajectory = isLeftTrajectory ? planetPathBisector * -asteroidSpeed : planetPathBisector * asteroidSpeed;
        _startPosition = isLeftTrajectory ? rightSideSpawnPosition : leftSideSpawnPosition;

        _asteroidTravelTime = Vector2.Distance(leftSideSpawnPosition, rightSideSpawnPosition) / asteroidSpeed;
        _waitInterval = 1.0f / asteroidsPerSecond;
        
        _asteroidSpawnRoutine = CoroutineManager.Instance.StartIndependentCoroutine(AsteroidSpawnRoutine());
        _asteroidDespawnRoutine = CoroutineManager.Instance.StartIndependentCoroutine(AsteroidDespawnRoutine());
    }

    private IEnumerator AsteroidSpawnRoutine()
    {
        while (true)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(_waitInterval);
        }
    }

    private IEnumerator AsteroidDespawnRoutine()
    {
        yield return new WaitForSeconds(_asteroidTravelTime);
        while (true)
        {
            DespawnOldestAsteroid();
            yield return new WaitForSeconds(_waitInterval);
        }
    }

    public void SpawnAsteroid()
    {
        var newAsteroid = ObjectPoolManager.Instance.AsteroidPool.Get();
        newAsteroid.Controller.Fire(_startPosition, _asteroidTrajectory);
        SpawnedAsteroids.Add(newAsteroid);
    }
    
    private void DespawnOldestAsteroid()
    {
        if (SpawnedAsteroids.Any())
        {
            ObjectPoolManager.Instance.AsteroidPool.Return(SpawnedAsteroids[0]);
            SpawnedAsteroids.RemoveAt(0);
        }
        else
        {
            Debug.Log("Can't despawn asteroids that do not exist!");
        }
    }

    public void RemoveAsteroidSpawner()
    {
        CoroutineManager.Instance.StopIndependentCoroutine(_asteroidSpawnRoutine);
        CoroutineManager.Instance.StopIndependentCoroutine(_asteroidDespawnRoutine);
        
        while (SpawnedAsteroids.Any())
        {
            DespawnOldestAsteroid();
        }
    }
}
