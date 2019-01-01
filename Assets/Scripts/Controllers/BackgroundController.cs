using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private static BackgroundController _instance;
    public static BackgroundController Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private GameObject _smallStarPrefab;
    [SerializeField]
    private GameObject _largeStarPrefab;

    [SerializeField]
    private Transform _smallStarParent;
    [SerializeField]
    private Transform _largeStarParent;

    [SerializeField]
    private float _smallStarParallaxSpeed = 1.0f;
    [SerializeField]
    private float _largeStarParallaxSpeed = 0.25f;

    private const float kHorizontalStarSpawnDistanceBuffer = 2.0f;

    private float _minimumSpawnX;
    private float _maximumSpawnX;

    [SerializeField]
    private int _minimumSmallStarsOnScreen = 15;
    [SerializeField]
    private int _maximumSmallStarsOnScreen = 30;

    float _minSpawnYDistanceSmallStars;
    float _maxSpawnYDistanceSmallStars;

    [SerializeField]
    private int _minimumLargeStarsOnScreen = 4;
    [SerializeField]
    private int _maximumLargeStarsOnScreen = 8;

    float _minSpawnYDistanceLargeStars;
    float _maxSpawnYDistanceLargeStars;

    [SerializeField]
    private float _minimumXSeperation = 20.0f;

    private Vector2 _currentSmallStarSpawnLocation;
    private Vector3 _currentLargeStarSpawnLocation;

    private List<StarData> _smallStarList;
    private List<StarData> _largeStarList;

    private Transform _cameraTransform;

    private Coroutine _smallStarRepositionCoroutine;
    private Coroutine _largeStarRepositionCoroutine;

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
        InitialiseStars();
        _smallStarParent.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -_smallStarParallaxSpeed);
        _largeStarParent.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -_largeStarParallaxSpeed);
    }

    private void InitialiseStars()
    {
        _cameraTransform = ScreenManager.Instance.MainCamera.transform;

        _minimumSpawnX = ScreenManager.Instance.MinOnScreenWorldX + kHorizontalStarSpawnDistanceBuffer;
        _maximumSpawnX = ScreenManager.Instance.MaxOnScreenWorldX - kHorizontalStarSpawnDistanceBuffer;

        float initialMinimumSpawnY = ScreenManager.Instance.CurrentMinScreenWorldY;
        float initialMaximumSpawnY = ScreenManager.Instance.CurrentMaxScreenWorldY;
        float verticalScreenSpace = initialMaximumSpawnY - initialMinimumSpawnY;

        InitialiseSmallStarBackground(initialMinimumSpawnY, verticalScreenSpace);
        InitialiseLargeStarBackground(initialMinimumSpawnY, verticalScreenSpace);
    }

    #region SmallStarLogic
    private void InitialiseSmallStarBackground(float minSpawnY, float verticalScreenSpace)
    {
        _smallStarList = new List<StarData>();

        _minSpawnYDistanceSmallStars = verticalScreenSpace / _maximumSmallStarsOnScreen;
        _maxSpawnYDistanceSmallStars = verticalScreenSpace / _minimumSmallStarsOnScreen;

        _currentSmallStarSpawnLocation = new Vector2 (0.0f, minSpawnY - _maxSpawnYDistanceSmallStars); //Initial spawn point just below screen.

        while (_smallStarList.Count < _maximumSmallStarsOnScreen + 1)
        {
            float yIncrement = CalculateNextRandomSmallStarLocation();

            GameObject smallStar = Instantiate(_smallStarPrefab, new Vector3(_currentSmallStarSpawnLocation.x, _currentSmallStarSpawnLocation.y, 1.0f), 
                _smallStarParent.transform.rotation, _smallStarParent);
            _smallStarList.Add(new StarData(smallStar, new Vector2(_currentSmallStarSpawnLocation.x, yIncrement)));

            float randomAnimationStart = Random.Range(0.0f, 1.0f);
            smallStar.GetComponent<Animator>().Play("StarSmallAnimation", 0, randomAnimationStart);
        }

        _smallStarRepositionCoroutine = StartCoroutine(RepositionSmallStarsRoutine());
    }

    // Returns how much Y was changed since the last small star location
    private float CalculateNextRandomSmallStarLocation()
    {
        float yIncrement = Random.Range(_minSpawnYDistanceSmallStars, _maxSpawnYDistanceSmallStars);
        _currentSmallStarSpawnLocation.x = PseudoRandomXPosition(_currentSmallStarSpawnLocation.x);
        _currentSmallStarSpawnLocation.y += yIncrement;

        return yIncrement;
    }

    private IEnumerator RepositionSmallStarsRoutine()
    {
        int index = 0;
        float nextRepositionDistance = _smallStarList[index].PositionVariation.y;

        while (true)
        {
            if (CameraSmallStarBackgroundSeperation >= nextRepositionDistance)
            {
                float yIncrement = CalculateNextRandomSmallStarLocation();
                var starToMove = _smallStarList[index];
                starToMove.StarObject.transform.localPosition = _currentSmallStarSpawnLocation;
                starToMove.PositionVariation = new Vector2(_currentSmallStarSpawnLocation.x, yIncrement);

                index = (index + 1) % _smallStarList.Count;
                nextRepositionDistance += _smallStarList[index].PositionVariation.y;
            }
            yield return null;
        }
    }
    
    private float CameraSmallStarBackgroundSeperation
    {
        get { return _cameraTransform.position.y - _smallStarParent.transform.position.y; }
    }
    #endregion

    #region LargeStarLogic
    private void InitialiseLargeStarBackground(float minSpawnY, float verticalScreenSpace)
    {
        _largeStarList = new List<StarData>();

        _minSpawnYDistanceLargeStars = verticalScreenSpace / _maximumLargeStarsOnScreen;
        _maxSpawnYDistanceLargeStars = verticalScreenSpace / _minimumLargeStarsOnScreen;

        _currentLargeStarSpawnLocation = new Vector2(0.0f, minSpawnY - _maxSpawnYDistanceLargeStars); //Initial spawn point just below screen.

        while (_largeStarList.Count < _maximumLargeStarsOnScreen + 1)
        {
            float yIncrement = CalculateNextRandomLargeStarLocation();

            GameObject largeStar = Instantiate(_largeStarPrefab, new Vector3(_currentLargeStarSpawnLocation.x, _currentLargeStarSpawnLocation.y, 1.0f),
                _largeStarParent.transform.rotation, _largeStarParent);
            _largeStarList.Add(new StarData(largeStar, new Vector2(_currentLargeStarSpawnLocation.x, yIncrement)));

            float randomAnimationStart = Random.Range(0.0f, 1.0f);
            largeStar.GetComponent<Animator>().Play("StarLargeAnimation", 0, randomAnimationStart);
        }

        _largeStarRepositionCoroutine = StartCoroutine(RepositionLargeStarsRoutine());
    }

    // Returns how much Y was changed since the last large star location
    private float CalculateNextRandomLargeStarLocation()
    {
        float yIncrement = Random.Range(_minSpawnYDistanceLargeStars, _maxSpawnYDistanceLargeStars);
        _currentLargeStarSpawnLocation.x = PseudoRandomXPosition(_currentLargeStarSpawnLocation.x);
        _currentLargeStarSpawnLocation.y += yIncrement;

        return yIncrement;
    }

    private IEnumerator RepositionLargeStarsRoutine()
    {
        int index = 0;
        float nextRepositionDistance = _largeStarList[index].PositionVariation.y;

        while (true)
        {
            if (CameraLargeStarBackgroundSeperation >= nextRepositionDistance)
            {
                float yIncrement = CalculateNextRandomLargeStarLocation();
                var starToMove = _largeStarList[index];
                starToMove.StarObject.transform.localPosition = _currentLargeStarSpawnLocation;
                starToMove.PositionVariation = new Vector2(_currentLargeStarSpawnLocation.x, yIncrement);

                index = (index + 1) % _largeStarList.Count;
                nextRepositionDistance += _largeStarList[index].PositionVariation.y;
            }
            yield return null;
        }
    }

    private float CameraLargeStarBackgroundSeperation
    {
        get { return _cameraTransform.position.y - _largeStarParent.transform.position.y; }
    }
    #endregion

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
}
