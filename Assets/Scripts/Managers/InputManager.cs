using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private enum ControlScheme
    {
        SwipeToMove,
        OneTouch,
        OneTouchMoveToTap
    }

    private Camera _mainCamera;
    private ShipController _shipController;
    private Vector3 _firstTouch;   //First touch position
    private Vector3 _lastTouch;   //Last touch position
    private bool _waitingForTouchCommand = false;
    private ControlScheme _currentControlScheme;
    
    private float _currentControlSchemeChangeHoldTime;
    private const float ControlSchemeChangeHoldTime = 1.0f;

    [SerializeField]
    private float dragDistance;  //minimum distance for a swipe to be registered

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
        _mainCamera = Camera.main;
        _currentControlSchemeChangeHoldTime = ControlSchemeChangeHoldTime;
#if UNITY_EDITOR
        _currentControlScheme = ControlScheme.SwipeToMove;
#else
        _currentControlScheme = ControlScheme.SwipeToMove;
#endif
        _shipController = ShipController.Instance;
    }

    void Update()
    {
        HandleMouseInput();
        HandleShieldInputs();
        
        if (Input.touchCount == 2) // user is touching the screen with a single touch
        {
            if (_currentControlSchemeChangeHoldTime > 0.0f)
            {
                _currentControlSchemeChangeHoldTime -= Time.deltaTime;
            }
            else
            {
                _currentControlScheme = ControlScheme.OneTouchMoveToTap;
            }
        }
        else if (Input.touchCount == 3)
        {
            if (_currentControlSchemeChangeHoldTime > 0.0f)
            {
                _currentControlSchemeChangeHoldTime -= Time.deltaTime;
            }
            else
            {
                _currentControlScheme = ControlScheme.SwipeToMove;
            }
        }
        else if (Input.touchCount == 4)
        {
            if (_currentControlSchemeChangeHoldTime > 0.0f)
            {
                _currentControlSchemeChangeHoldTime -= Time.deltaTime;
            }
            else
            {
                _currentControlScheme = ControlScheme.OneTouch;
            }
        }
        else
        {
            _currentControlSchemeChangeHoldTime = ControlSchemeChangeHoldTime;
        }
    }

    private void HandleMouseInput()
    {
        switch (_currentControlScheme)
        {
            case ControlScheme.SwipeToMove:
                HandleInputForSwipeToMoveControlScheme();
                break;
            case ControlScheme.OneTouch:
            case ControlScheme.OneTouchMoveToTap:
                HandleInputForOneTouchControlScheme();
                break;
        }
    }

    private void HandleInputForSwipeToMoveControlScheme()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _waitingForTouchCommand = true;
            _firstTouch = Input.mousePosition;
            _lastTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            _lastTouch = Input.mousePosition;

            if (_waitingForTouchCommand)
            {
                CheckForSwipeInput();
            }
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_waitingForTouchCommand)
            {
                // Tap input was succesfully registered
                InputSingleTap();
            }
        }
    }

    private void HandleInputForOneTouchControlScheme()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InputSingleTap();
        }
    }

    private void InputSingleTap()
    {
        if (GameStateManager.Instance.IsGameInProgress)
        {
            Vector3? tapPosition = null;
            if (_currentControlScheme != ControlScheme.SwipeToMove && _shipController.IsOrbiting)
            {
                if (_currentControlScheme == ControlScheme.OneTouchMoveToTap)
                {
                    var screenToWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    tapPosition = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
                }
                _shipController.TapLeaveOrbit(tapPosition);
            }
            else
            {
                _shipController.SwitchShieldState();
            }
        }
        else if (GameStateManager.Instance.IsGameOver)
        {
            // TODO - Don't use this garbage scene reloading and properly return to pool and reposition.
            SceneManager.LoadScene(0);
        }
    }

    private void CheckForSwipeInput()
    {
        if (Mathf.Abs(_lastTouch.x - _firstTouch.x) > dragDistance || Mathf.Abs(_lastTouch.y - _firstTouch.y) > dragDistance)
        {
            // Swipe input was succesfully registered
            Vector2 swipeDirection = new Vector2(_lastTouch.x - _firstTouch.x, _lastTouch.y - _firstTouch.y);
            swipeDirection.Normalize();
            _shipController.MoveShip(swipeDirection);
            _waitingForTouchCommand = false;
        }
    }


    private void HandleShieldInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _shipController.ActivateShield();
        }
    }
}
