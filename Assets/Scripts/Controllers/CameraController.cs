using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _playerTransform;
    private Transform _cameraTransform;
    private Vector3 _initialOffset;
    private Vector3 _newCameraPosition;

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
        InitialiseCameraPosition();
    }

    public void InitialiseCameraPosition()
    {
        _playerTransform = ShipController.Instance.PlayerTransform;
        _cameraTransform = GetComponent<Transform>();
        _initialOffset = _playerTransform.position - _cameraTransform.position;
        _newCameraPosition = _cameraTransform.position;
    }

    private void Update()
    {
        Vector2 offsetPlayerPosition = _playerTransform.position - _initialOffset;
        if (_newCameraPosition.y < offsetPlayerPosition.y)
        {
            _newCameraPosition.y = offsetPlayerPosition.y;
            _cameraTransform.position = _newCameraPosition;
        }
    }
}
