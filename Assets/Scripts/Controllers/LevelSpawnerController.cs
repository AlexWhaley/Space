using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class LevelSpawnerController : MonoBehaviour
{
    private static LevelSpawnerController _instance;
    public static LevelSpawnerController Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField, Range(0, 1)]
    private float _smallToBigPlanetRatio = 0.25f;
    [SerializeField]
    private Transform _planetParent;

    [SerializeField]
    private GameObject _bigPlanetPrefab;
    [SerializeField]
    private GameObject _smallPlanetPrefab;

    private int _maxPlanetCount;

    [SerializeField, Range(0, 1)]
    float _minSpawnYDistanceScreenPercentage = 0.45f;
    [SerializeField, Range(0, 1)]
    float _maxSpawnYDistanceScreenPercentage = 0.75f;
    float _minSpawnYDistancePlanets;
    float _maxSpawnYDistancePlanets;
    float _verticalScreenSpace;
    private const float kHorizontalSpawnBuffer = 11.5f;
    private float _minimumSpawnX;
    private float _maximumSpawnX;

    [SerializeField]
    private float _minimumXSeperation = 10.0f;
    private Vector3 _currentPlanetSpawnLocation = new Vector3(0.0f, 0.0f, 1.0f);

    [SerializeField] 
    private float _planetOrbitObstacleRotationMin = 2.0f;
    [SerializeField] 
    private float _planetOrbitObstacleRotationMax = 4.0f;

    private List<PlanetData> _planetList;
    private List<PlanetData> _smallPlanetPool;
    private List<PlanetData> _bigPlanetPool;

    private Transform _cameraTransform;
    private Coroutine _sectionSpawnRoutine;

    private bool _isLeftTrajectory;

    private void Awake()
    {
        // Wont need this just for clarification.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // If the singleton hasn't been initialized yet
            _instance = this;
        }
    }

    private void Start()
    {
        InitialisePlanets();
    }

    private void InitialisePlanets()
    {
        _cameraTransform = ScreenManager.Instance.MainCamera.transform;
        _minimumSpawnX = ScreenManager.Instance.MinOnScreenWorldX + kHorizontalSpawnBuffer;
        _maximumSpawnX = ScreenManager.Instance.MaxOnScreenWorldX - kHorizontalSpawnBuffer;

        _verticalScreenSpace = ScreenManager.Instance.CurrentMaxScreenWorldY - ScreenManager.Instance.CurrentMinScreenWorldY;
        _minSpawnYDistancePlanets = _verticalScreenSpace * _minSpawnYDistanceScreenPercentage;
        _maxSpawnYDistancePlanets = _verticalScreenSpace * _maxSpawnYDistanceScreenPercentage;

        // + 4 -> one for interaction on either side of the maximum poissible on screen planets.
        _maxPlanetCount = (int)(1.0f / _minSpawnYDistanceScreenPercentage) + 4;

        CreatePlanetPools();

        _planetList = new List<PlanetData>(_maxPlanetCount);

        //Initial spawn point centred up and onscreen from player
        _currentPlanetSpawnLocation.y += (0.25f * _verticalScreenSpace);

        PlanetData initialPlanet = GetPooledBigPlanet();
        initialPlanet.PositionVariation = new Vector2(_currentPlanetSpawnLocation.x, _currentPlanetSpawnLocation.y);
        initialPlanet.PlanetController.SetupPlanet(_currentPlanetSpawnLocation, SpriteManager.Instance.GetRandomPlanetSprite(initialPlanet.IsSmallPlanet), GetRandomOrbitObstacleRotationValue);

        _planetList.Add(initialPlanet);

        while (_planetList.Count < _maxPlanetCount)
        {
            AddNextRandomPlanet();
        }

        _sectionSpawnRoutine = StartCoroutine(SectionSpawnRoutine());
    }

    private float GetRandomOrbitObstacleRotationValue => Random.Range(_planetOrbitObstacleRotationMin, _planetOrbitObstacleRotationMax);

    private void CreatePlanetPools()
    {
        Vector3 offScreenSpawn = new Vector3(0.0f, -100.0f, 1.0f);
        _smallPlanetPool = new List<PlanetData>(_maxPlanetCount);
        _bigPlanetPool = new List<PlanetData>(_maxPlanetCount);

        var parentRotation = _planetParent.transform.rotation;

        for (int i = 0; i < _maxPlanetCount; ++i)
        {
            GameObject smallPlanet = Instantiate(_smallPlanetPrefab, offScreenSpawn, parentRotation, _planetParent);
            GameObject bigPlanet = Instantiate(_bigPlanetPrefab, offScreenSpawn, parentRotation, _planetParent);
            _smallPlanetPool.Add(new PlanetData(smallPlanet, offScreenSpawn, true));
            _bigPlanetPool.Add(new PlanetData(bigPlanet, offScreenSpawn, false));
        }
    }

    private void AddNextRandomPlanet()
    {
        // TODO - When changing the object pooling return to pool here
        if (_planetList.Count == _maxPlanetCount)
        {
            _planetList[0].CleanUpObstacles();
            _planetList.RemoveAt(0);
        }

        float yIncrement = CalculateNextRandomPlanetLocation();

        PlanetData planet = Random.Range(0.0f, 1.0f) <= _smallToBigPlanetRatio ? GetPooledSmallPlanet() : GetPooledBigPlanet();
        planet.PositionVariation = new Vector2(_currentPlanetSpawnLocation.x, yIncrement);
        
        // TODO - When object pooled the getter should fetch the random sprite
        planet.PlanetController.SetupPlanet(_currentPlanetSpawnLocation, SpriteManager.Instance.GetRandomPlanetSprite(planet.IsSmallPlanet), GetRandomOrbitObstacleRotationValue);

        _planetList.Add(planet);
        CreatePlanetObstacles(planet,1.0f);
    }

    // Returns how much Y was changed since the last small star location
    private float CalculateNextRandomPlanetLocation()
    {
        float yIncrement = Random.Range(_minSpawnYDistancePlanets, _maxSpawnYDistancePlanets);
        _currentPlanetSpawnLocation.x = PseudoRandomXPosition(_currentPlanetSpawnLocation.x);
        _currentPlanetSpawnLocation.y += yIncrement;

        return yIncrement;
    }

    private void CreatePlanetObstacles(PlanetData newPlanet, float difficultyModifier)
    {
        CreateBeltObstacles(newPlanet, difficultyModifier);
        CreateOrbitItems(newPlanet);
        _isLeftTrajectory = !_isLeftTrajectory;
    }

    private void CreateOrbitItems(PlanetData newPlanet)
    {
        var obstacleCount = Random.Range(2, 5);
        var orbitObstacleSpawner = new OrbitObstacleSpawner(newPlanet, obstacleCount, 0);
        newPlanet.ObstacleSpawners.Add(orbitObstacleSpawner);
    }

    private void CreateBeltObstacles(PlanetData newPlanet, float difficultyModifier)
    {
        var previousPlanet = _planetList[_planetList.Count - 2];
        var previousPlanetPosition = previousPlanet.PlanetObject.transform.position;
        var normalizedPlanetPath = (_currentPlanetSpawnLocation - previousPlanetPosition).normalized;
        var obstacleBeltCount = Random.Range(1, 2);

        Vector2 useablePathMin = previousPlanetPosition + (previousPlanet.OrbitRadius + 1.0f) * normalizedPlanetPath;
        Vector2 useablePathMax = _currentPlanetSpawnLocation - (newPlanet.OrbitRadius + 1.0f) * normalizedPlanetPath;
        
        for (int i = 1; i <= obstacleBeltCount; ++i)
        {
            var spawnPosition = useablePathMin + (useablePathMax - useablePathMin) * i / (obstacleBeltCount + 1);
            CreateBelt(newPlanet, spawnPosition, normalizedPlanetPath);
        }
    }

    private void CreateBelt(PlanetData planet, Vector2 spawnPosition, Vector2 planetPath)
    {
        var asteroidSpawner = new AsteroidSpawner(spawnPosition, planetPath, _isLeftTrajectory, 10.0f,1.0f, 1.5f);
        planet.ObstacleSpawners.Add(asteroidSpawner);
    }

    private IEnumerator SectionSpawnRoutine()
    {
        float nextRepositionDistance = _planetList[0].PositionVariation.y;

        while (true)
        {
            if (CameraPlanetParentAdjustedSeperation >= nextRepositionDistance)
            {
                // We delete the oldest entry and create a new one.
                AddNextRandomPlanet();
                nextRepositionDistance += _planetList[0].PositionVariation.y;
            }
            yield return null;
        }
    }

    private float CameraPlanetParentAdjustedSeperation
    {
        get { return (_cameraTransform.position.y - _planetParent.transform.position.y) - _verticalScreenSpace; }
    }

    private float PseudoRandomXPosition(float previousXPosition)
    {
        float leftSpawnUpperBound = previousXPosition - _minimumXSeperation;
        float rightSpawnLowerBound = previousXPosition + _minimumXSeperation;

        if (leftSpawnUpperBound < _minimumSpawnX)
        {
            leftSpawnUpperBound = _minimumSpawnX;
        }
        if (rightSpawnLowerBound > _maximumSpawnX)
        {
            rightSpawnLowerBound = _maximumSpawnX;
        }

        float leftSpawnRangeSize = leftSpawnUpperBound - _minimumSpawnX;
        float rightSpawnRangeSize = _maximumSpawnX - rightSpawnLowerBound;
        float totalRangeSize = leftSpawnRangeSize + rightSpawnRangeSize;

        float leftSpawnProportion = leftSpawnRangeSize / totalRangeSize;

        float pseudoRandomXPos = Random.Range(0.0f, 1.0f) <= leftSpawnProportion ? Random.Range(_minimumSpawnX, leftSpawnUpperBound) : Random.Range(rightSpawnLowerBound, _maximumSpawnX);
        return pseudoRandomXPos;
    }

    private PlanetData GetPooledSmallPlanet()
    {
        _smallPlanetPool.MoveItemAtFrontToEnd();
        return _smallPlanetPool[0];
    }

    private PlanetData GetPooledBigPlanet()
    {
        _bigPlanetPool.MoveItemAtFrontToEnd();
        return _bigPlanetPool[0];
    }
}
