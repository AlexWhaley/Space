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

    private List<PlanetData> _planetList;
    private List<PlanetData> _smallPlanetPool;
    private List<PlanetData> _bigPlanetPool;

    private Transform _cameraTransform;
    private Coroutine _sectionSpawnRoutine;

    // Additional Connecting Obstacles
    public enum ConnectingObstacleType
    {
        Asteroids
    }

    private void Awake()
    {
        // Wont need this just for clarification.
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
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

        // + 2 -> one for interaction on either side of the maximum poissible on screen planets.
        _maxPlanetCount = (int)(1.0f / _minSpawnYDistanceScreenPercentage) + 3;

        CreatePlanetPools();

        _planetList = new List<PlanetData>(_maxPlanetCount);

        //Initial spawn point centred up and onscreen from player
        _currentPlanetSpawnLocation.y += (0.25f * _verticalScreenSpace);

        PlanetData initialPlanet = GetPooledBigPlanet();
        initialPlanet.PositionVariation = new Vector2(_currentPlanetSpawnLocation.x, _currentPlanetSpawnLocation.y);
        initialPlanet.PlanetController.Initialise(_currentPlanetSpawnLocation, SpriteManager.Instance.GetRandomPlanetSprite());

        _planetList.Add(initialPlanet);

        while (_planetList.Count < _maxPlanetCount)
        {
            AddNextRandomPlanet();
        }

        _sectionSpawnRoutine = StartCoroutine(SectionSpawnRoutine());
    }

    private void CreatePlanetPools()
    {
        Vector3 offScreenSpawn = new Vector3(0.0f, -100.0f, 1.0f);
        _smallPlanetPool = new List<PlanetData>(_maxPlanetCount);
        _bigPlanetPool = new List<PlanetData>(_maxPlanetCount);

        for (int i = 0; i < _maxPlanetCount; ++i)
        {
            GameObject smallPlanet = Instantiate(_smallPlanetPrefab, offScreenSpawn, _planetParent.transform.rotation, _planetParent);
            GameObject bigPlanet = Instantiate(_bigPlanetPrefab, offScreenSpawn, _planetParent.transform.rotation, _planetParent);
            _smallPlanetPool.Add(new PlanetData(smallPlanet, offScreenSpawn));
            _bigPlanetPool.Add(new PlanetData(bigPlanet, offScreenSpawn));
        }

    }

    private void AddNextRandomPlanet()
    {
        if (_planetList.Count == _maxPlanetCount)
        {
            // Uninitialisation with controller done here
            _planetList.RemoveAt(0);
        }

        float yIncrement = CalculateNextRandomPlanetLocation();

        PlanetData planet = Random.Range(0.0f, 1.0f) <= _smallToBigPlanetRatio ? GetPooledSmallPlanet() : GetPooledBigPlanet();
        planet.PositionVariation = new Vector2(_currentPlanetSpawnLocation.x, yIncrement);
        planet.PlanetController.Initialise(_currentPlanetSpawnLocation, SpriteManager.Instance.GetRandomPlanetSprite());

        _planetList.Add(planet);
        
        var previousPlanetPosition = _planetList[_planetList.Count - 2].PlanetObject.transform.position;
        var spawnPosition = (previousPlanetPosition + _currentPlanetSpawnLocation) / 2;
        var planetPath = _currentPlanetSpawnLocation - previousPlanetPosition;
        
        var asteroidSpawner = new AsteroidSpawner(spawnPosition, planetPath, true, 15.0f,2.5f);
    }

    // Returns how much Y was changed since the last small star location
    private float CalculateNextRandomPlanetLocation()
    {
        float yIncrement = Random.Range(_minSpawnYDistancePlanets, _maxSpawnYDistancePlanets);
        _currentPlanetSpawnLocation.x = PseudoRandomXPosition(_currentPlanetSpawnLocation.x);
        _currentPlanetSpawnLocation.y += yIncrement;

        return yIncrement;
    }

    private void CreatePathObstacles()
    {

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
