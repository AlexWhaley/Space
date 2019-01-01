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

    [SerializeField]
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

    public enum ShieldState
    {
        Blue,
        Yellow
    }
    private ShieldState _currentShieldState = ShieldState.Blue;

    private bool _canSwipeMove = true;
    private Coroutine _shipOrbitCoroutine = null;
    
    // Constants
    private const string kSetBlueShieldState = "SetBlueState";
    private const string kSetYellowShieldState = "SetYellowState";
    private const string kActivateBlueShield = "UseBlueShield";
    private const string kActivateYellowShield = "UseYellowShield";
    private const float kDegreesInACircle = 360.0f;
    
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
        //Initialising Trail Colours
        _trailStartBlue = _blueTrailColor;
        _trailStartBlue.a = _shipTrail.startColor.a;
        _trailEndBlue = _blueTrailColor;
        _trailEndBlue.a = _shipTrail.endColor.a;

        _trailStartYellow = _yellowTrailColor;
        _trailStartYellow.a = _shipTrail.startColor.a;
        _trailEndYellow = _yellowTrailColor;
        _trailEndYellow.a = _shipTrail.endColor.a;

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
        switch (_currentShieldState)
        {
            case ShieldState.Blue:
                _currentShieldState = ShieldState.Yellow;
                _shipAnimator.SetTrigger(kSetYellowShieldState);
                break;

            case ShieldState.Yellow:
                _currentShieldState = ShieldState.Blue;
                _shipAnimator.SetTrigger(kSetBlueShieldState);
                break;
        }

        SetTrailColour();
    }

    private void SetTrailColour()
    {
        switch (_currentShieldState)
        {
            case ShieldState.Blue:
                _shipTrail.startColor = _trailStartBlue;
                _shipTrail.endColor = _trailEndBlue;
                break;

            case ShieldState.Yellow:
                _shipTrail.startColor = _trailStartYellow;
                _shipTrail.endColor = _trailEndYellow;
                break;
        }
    }

    public void ActivateShield()
    {
        switch (_currentShieldState)
        {
            case ShieldState.Blue:
                _shipAnimator.SetTrigger(kActivateBlueShield);
                break;

            case ShieldState.Yellow:
                _shipAnimator.SetTrigger(kActivateYellowShield);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case TagUtility.Orbit:
                BeginOrbit(other.transform, ((CircleCollider2D)other).radius);
                break;
        }
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

    public ShieldState CurrentShieldState
    {
        get { return _currentShieldState; }
        set { _currentShieldState = value; }
    }
}
