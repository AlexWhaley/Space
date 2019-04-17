using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner : IObstacleSpawner
{
    private const float OffscreenOffset = 2.0f;
    private Vector2 _planetPath;
    private Vector2 _startPosition;
    private Vector2 _asteroidTrajectory;
    private bool _isLeftTrajectory;
    private float _minWaitInterval;
    private float _maxWaitInterval;
    private float _asteroidTravelTime;

    private Queue<float> _spawnWaitTimes = new Queue<float>();
    private List<Asteroid> _spawnedAsteroids = new List<Asteroid>();
    private Coroutine _asteroidSpawnRoutine;
    private Coroutine _asteroidDespawnRoutine;

    public AsteroidSpawner(Vector2 spawnerPosition, Vector2 planetPath, bool isLeftTrajectory, float asteroidSpeed, float minAsteroidsPerSecond, float maxAsteroidsPerSecond)
    {
        _planetPath = planetPath;
        _isLeftTrajectory = isLeftTrajectory;
        // The effective equation of the line that bisects the line between the two planets
        var planetPathBisector = new Vector2(planetPath.y, -planetPath.x).normalized;
        
        float rightSpawnXDistanceOffset = CameraUtilities.CameraRightEdgeToWorld + OffscreenOffset - spawnerPosition.x;
        float rightSpawnYDistanceOffset = rightSpawnXDistanceOffset / planetPathBisector.x * planetPathBisector.y;
        Vector2 rightSideSpawnPosition = new Vector2(spawnerPosition.x + rightSpawnXDistanceOffset,spawnerPosition.y + rightSpawnYDistanceOffset);
        
        float leftSpawnXDistanceOffset = CameraUtilities.CameraLeftEdgeToWorld - OffscreenOffset - spawnerPosition.x;
        float leftSpawnYDistanceOffset = leftSpawnXDistanceOffset / planetPathBisector.x * planetPathBisector.y;
        Vector2 leftSideSpawnPosition = new Vector2(spawnerPosition.x + leftSpawnXDistanceOffset,spawnerPosition.y + leftSpawnYDistanceOffset);
        
        _asteroidTrajectory = _isLeftTrajectory ? planetPathBisector * -asteroidSpeed : planetPathBisector * asteroidSpeed;
        _startPosition = _isLeftTrajectory ? rightSideSpawnPosition : leftSideSpawnPosition;

        _asteroidTravelTime = Vector2.Distance(leftSideSpawnPosition, rightSideSpawnPosition) / asteroidSpeed;
        _minWaitInterval = 1.0f / minAsteroidsPerSecond;
        _maxWaitInterval = 1.0f / maxAsteroidsPerSecond;
        
        _asteroidSpawnRoutine = CoroutineManager.Instance.StartIndependentCoroutine(AsteroidSpawnRoutine());
        _asteroidDespawnRoutine = CoroutineManager.Instance.StartIndependentCoroutine(AsteroidDespawnRoutine());
    }

    private IEnumerator AsteroidSpawnRoutine()
    {
        while (true)
        {
            SpawnAsteroid();
            float randomisedWaitTime = Random.Range(_minWaitInterval, _maxWaitInterval);
            _spawnWaitTimes.Enqueue(randomisedWaitTime);
            yield return new WaitForSeconds(randomisedWaitTime);
        }
    }

    private IEnumerator AsteroidDespawnRoutine()
    {
        yield return new WaitForSeconds(_asteroidTravelTime);
        while (true)
        {
            DespawnOldestAsteroid();
            yield return new WaitForSeconds(_spawnWaitTimes.Dequeue());
        }
    }

    public void SpawnAsteroid()
    {
        var newAsteroid = ObjectPoolManager.Instance.AsteroidPool.Get();
        Vector3 startPosition = _startPosition + _planetPath * Random.Range(-1.0f, 1.0f) * 0.03f;
        newAsteroid.Controller.Fire(startPosition, _asteroidTrajectory, _isLeftTrajectory);
        _spawnedAsteroids.Add(newAsteroid);
    }
    
    private void DespawnOldestAsteroid()
    {
        if (_spawnedAsteroids.Any())
        {
            ObjectPoolManager.Instance.AsteroidPool.Return(_spawnedAsteroids[0]);
            _spawnedAsteroids.RemoveAt(0);
        }
        else
        {
            Debug.Log("Can't despawn asteroids that do not exist!");
        }
    }

    public void RemoveSpawner()
    {
        CoroutineManager.Instance.StopIndependentCoroutine(_asteroidSpawnRoutine);
        CoroutineManager.Instance.StopIndependentCoroutine(_asteroidDespawnRoutine);
        
        while (_spawnedAsteroids.Any())
        {
            DespawnOldestAsteroid();
        }
    }
}
