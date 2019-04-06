using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class ShipController : MonoBehaviour
{
    private static ShipController _instance;
    public static ShipController Instance
    {
        get
        {
            return _instance;
        }
    }

    private Rigidbody2D _playerRB;
    public Transform PlayerTransform
    {
        get { return _playerRB.transform; }
    }

    [SerializeField]
    private Animator _shipAnimator;
    [SerializeField]
    private float _shipSpeed = 30.0f;

    [Header("Trail Properties")]
    [SerializeField]
    private TrailRenderer _shipTrail;
    [SerializeField]
    private Color _blueTrailColor;
    [SerializeField]
    private Color _yellowTrailColor;

    private Color _trailStartBlue;
    private Color _trailEndBlue;
    private Color _trailStartYellow;
    private Color _trailEndYellow;

    private ObstacleColour _currentShieldColour = ObstacleColour.Blue;

    private bool _canSwipeMove = true;
    private Coroutine _shipOrbitCoroutine;
    
    // Constants
    private const string kSetBlueShieldState = "SetBlueState";
    private const string kSetYellowShieldState = "SetYellowState";
    private const string kActivateBlueShield = "UseBlueShield";
    private const string kActivateYellowShield = "UseYellowShield";
    private const float kDegreesInACircle = 360.0f;

    [SerializeField] private GameObject _shipObject;
    [SerializeField] private ColliderObject _orbitCollider;
    
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
            Initialise();
        }
    }

    private void Initialise()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Initialising Trail Colours
        _trailStartBlue = _blueTrailColor;
        _trailStartBlue.a = _shipTrail.startColor.a;
        _trailEndBlue = _blueTrailColor;
        _trailEndBlue.a = _shipTrail.endColor.a;

        _trailStartYellow = _yellowTrailColor;
        _trailStartYellow.a = _shipTrail.startColor.a;
        _trailEndYellow = _yellowTrailColor;
        _trailEndYellow.a = _shipTrail.endColor.a;

        _orbitCollider.OnTriggerEnter = OnOrbitTriggerEnter;
        
        SetTrailColour();
    }

    private Vector3 ShipForwardDirection
    {
        get { return _playerRB.transform.up; }
        set { _playerRB.transform.up = value; }
    }

    public void SwipeMoveShip(Vector2 swipeDirection)
    {
        if (_canSwipeMove)
        {
            EndOrbitIfExists();
            Vector2 swipeForce = swipeDirection * _shipSpeed;
            _playerRB.velocity = swipeForce;
            ShipForwardDirection = swipeDirection;
            _canSwipeMove = false;
        }
    }

    public void SwitchShieldState()
    {
        switch (_currentShieldColour)
        {
            case ObstacleColour.Blue:
                _currentShieldColour = ObstacleColour.Yellow;
                _shipAnimator.SetTrigger(kSetYellowShieldState);
                break;

            case ObstacleColour.Yellow:
                _currentShieldColour = ObstacleColour.Blue;
                _shipAnimator.SetTrigger(kSetBlueShieldState);
                break;
        }

        SetTrailColour();
    }

    private void SetTrailColour()
    {
        switch (_currentShieldColour)
        {
            case ObstacleColour.Blue:
                _shipTrail.startColor = _trailStartBlue;
                _shipTrail.endColor = _trailEndBlue;
                break;

            case ObstacleColour.Yellow:
                _shipTrail.startColor = _trailStartYellow;
                _shipTrail.endColor = _trailEndYellow;
                break;
        }
    }

    public void ActivateShield()
    {
        switch (_currentShieldColour)
        {
            case ObstacleColour.Blue:
                _shipAnimator.SetTrigger(kActivateBlueShield);
                break;

            case ObstacleColour.Yellow:
                _shipAnimator.SetTrigger(kActivateYellowShield);
                break;
        }
    }

    private void OnOrbitTriggerEnter(Collider2D other)
    {
        BeginOrbit(other.transform, ((CircleCollider2D)other).radius);
    }

    private void BeginOrbit(Transform orbitObject, float orbitRadius)
    {
        // Set velocity to zero since the ships position will now be controlled directly via rotation.
        _playerRB.velocity = Vector3.zero;
        
        Vector3 directionToPlayer = _playerRB.transform.position - orbitObject.position;
        _playerRB.transform.position = orbitObject.position + (directionToPlayer.normalized * orbitRadius);

        float angle = Vector3.Angle(ShipForwardDirection, (directionToPlayer * -1));

        bool shouldOrbitAntiClockwise = ShipForwardDirection.AngleDir(directionToPlayer * -1) > 0;
        float rotationAngle = shouldOrbitAntiClockwise ? (90 - angle) * 1.0f : (90 - angle) * -1.0f;

        Vector3 tangentRotation = new Vector3(0,0, rotationAngle);
        _playerRB.transform.Rotate(tangentRotation);

        _shipOrbitCoroutine = StartCoroutine(OrbitAroundPlanet(orbitObject, orbitRadius, !shouldOrbitAntiClockwise));
        _canSwipeMove = true;
    }

    public void DestroyPlayer()
    {
        // Spawn player destroyed effects.
        _shipObject.SetActive(false);
        _playerRB.velocity = Vector2.zero;
        _playerRB.inertia = 0.0f;
    }

    private IEnumerator OrbitAroundPlanet(Transform orbitObject, float orbitRadius, bool isRotatingClockwise)
    {
        float degreesPerSecond = _shipSpeed / ((Mathf.PI * 2 * orbitRadius) / kDegreesInACircle);
        if (!isRotatingClockwise)
        {
            degreesPerSecond *= -1;
        }


        while (true)
        {
            _playerRB.transform.RotateAround(orbitObject.transform.position, Vector3.forward, Time.fixedDeltaTime * degreesPerSecond);
            yield return new WaitForFixedUpdate();
        }
    }

    private void EndOrbitIfExists()
    {
        if (_shipOrbitCoroutine != null)
        {
            StopCoroutine(_shipOrbitCoroutine);
            _shipOrbitCoroutine = null;
        }
    }

    public ObstacleColour CurrentShieldColour
    {
        get { return _currentShieldColour; }
        set { _currentShieldColour = value; }
    }
}
