using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtilities
{
    private static Camera _mainCamera;
    private static float? _cameraLeftEdgeWorld;
    private static float? _cameraRightEdgeWorld;
    
    public static Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            return _mainCamera;
        }
    }

    public static float CameraLeftEdgeToWorld
    {
        get
        {
            if (!_cameraLeftEdgeWorld.HasValue)
            {
                _cameraLeftEdgeWorld = MainCamera.ScreenToWorldPoint(new Vector3(0, 0,0)).x;
            }
            return _cameraLeftEdgeWorld.Value;
        }
    }
    
    public static float CameraRightEdgeToWorld
    {
        get
        {
            if (!_cameraRightEdgeWorld.HasValue)
            {
                _cameraRightEdgeWorld = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0,0)).x;
            }
            return _cameraRightEdgeWorld.Value;
        }
    }
}
