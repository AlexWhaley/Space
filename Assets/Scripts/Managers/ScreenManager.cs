using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager _instance;
    public static ScreenManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera
    {
        get { return _mainCamera; }
        private set { _mainCamera = value; }
    }

    private Vector3 _minimumViewportCoord;
    private Vector3 _maximumViewportCoord;

    // Since horizontal screen space doesn't change these are fixed.
    public float MinOnScreenWorldX { get; private set; }
    public float MaxOnScreenWorldX { get; private set; }

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
        _minimumViewportCoord = new Vector3(0.0f, 0.0f, _mainCamera.nearClipPlane);
        _maximumViewportCoord = new Vector3(1.0f, 1.0f, _mainCamera.nearClipPlane);

        MinOnScreenWorldX = _mainCamera.ViewportToWorldPoint(_minimumViewportCoord).x;
        MaxOnScreenWorldX = _mainCamera.ViewportToWorldPoint(_maximumViewportCoord).x;
    }

    // Vertical world space changes so these valeus are not fixed.
    public float CurrentMinScreenWorldY
    {
        get { return _mainCamera.ViewportToWorldPoint(_minimumViewportCoord).y; }
    }
    public float CurrentMaxScreenWorldY
    {
        get { return _mainCamera.ViewportToWorldPoint(_maximumViewportCoord).y; }
    }

}
