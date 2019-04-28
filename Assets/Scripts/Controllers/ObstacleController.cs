using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
public class ObstacleController : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected Rigidbody2D _rigidBody;
    protected Collider2D _collider2D;
    protected ObstacleColour _colour;
    protected Color _originalSpriteColour;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _originalSpriteColour = _spriteRenderer.color;
    }

    public virtual void SetColour(ObstacleColour colour)
    {
        _colour = colour;
    }

    protected virtual void DestroyObstacle(Vector2 playerTrajectory){}

    public void HideObstacle()
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.inertia = 0.0f;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        _collider2D.enabled = false;
        _spriteRenderer.color = Color.clear;
    }
    
    public void RevealObstacle()
    {
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _collider2D.enabled = true;
        _spriteRenderer.color = _originalSpriteColour;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        int triggerLayer = other.gameObject.layer;
        
        if (triggerLayer == PhysicsLayers.Shield)
        {
            var shipController = other.gameObject.GetComponentInParent<ShipController>();
            
            if (shipController.CurrentShieldColour == _colour)
            {
                CollideWithPlayer(shipController.PlayerTransform.up);
            }
        }
        else if (triggerLayer == PhysicsLayers.PlayerObstacle)
        {
            var shipController = other.gameObject.GetComponentInParent<ShipController>();
            if (shipController.CurrentShieldColour != _colour)
            {
                shipController.DestroyPlayer();
            }
        }
    }
    
    public ObstacleColour Colour
    {
        get { return _colour; }
    }

    public void CollideWithPlayer(Vector2 playerTrajectory)
    {
       // Spawn collision elements
       DestroyObstacle(playerTrajectory);
       HideObstacle();
    }
}
