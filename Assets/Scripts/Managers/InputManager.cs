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

    private Vector3 _firstTouch;   //First touch position
    private Vector3 _lastTouch;   //Last touch position
    private bool _waitingForTouchCommand = false;

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

    void Update()
    {
        HandleMouseInput();
        HandleShieldInputs();
        /**
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                _firstTouch = touch.position;
                _lastTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                _lastTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                _lastTouch = touch.position;  //last touch position. Ommitted if you use list
            }
            
            if (Mathf.Abs(_lastTouch.x - _firstTouch.x) > dragDistance || Mathf.Abs(_lastTouch.y - _firstTouch.y) > dragDistance)
            {
                Vector2 swipeDirection = new Vector2(_lastTouch.x - _firstTouch.x, _lastTouch.y - _firstTouch.y);
                swipeDirection.Normalize();
                ShipController.Instance.SwipeMoveShip(swipeDirection, 100.0f);
            }
            else
            {   //It's a tap as the drag distance is less than 20% of the screen height
                Debug.Log("Tap");
            }
        }
    **/
    }

    private void HandleMouseInput()
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
                if (GameStateManager.Instance.IsGameInProgress)
                {
                    ShipController.Instance.SwitchShieldState();
                }
                else if (GameStateManager.Instance.IsGameOver)
                {
                    // TODO - Don't use this garbage scene reloading and properly return to pool and reposition.
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    private void CheckForSwipeInput()
    {
        if (Mathf.Abs(_lastTouch.x - _firstTouch.x) > dragDistance || Mathf.Abs(_lastTouch.y - _firstTouch.y) > dragDistance)
        {
            // Swipe input was succesfully registered
            Vector2 swipeDirection = new Vector2(_lastTouch.x - _firstTouch.x, _lastTouch.y - _firstTouch.y);
            swipeDirection.Normalize();
            ShipController.Instance.SwipeMoveShip(swipeDirection);
            _waitingForTouchCommand = false;
        }
    }


    private void HandleShieldInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShipController.Instance.ActivateShield();
        }
    }
}
