using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : ObstacleController
{
    public override void SetColour(ObstacleColour colour)
    {
        base.SetColour(colour);
        _spriteRenderer.sprite = SpriteManager.Instance.GetRandomAsteroidSprite(_colour);
    }
    
    public void Fire(Vector3 startPosition, Vector3 velocity, bool _isLeftTrajectory)
    {
        transform.position = startPosition;
        _rigidBody.velocity = velocity;
        _rigidBody.angularVelocity = _isLeftTrajectory ? Random.Range(25.0f, 50.0f) : Random.Range(-50.0f, -25.0f);
    }
}
