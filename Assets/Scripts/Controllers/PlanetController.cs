using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public Transform OrbitTransform;
    private SpriteRenderer _spriteRenderer;

    private bool _rotateClockwise;
    private float _rotationSpeed;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetupPlanet(Vector3 newPosition, Sprite newSprite, float rotationSpeed)
    {
        transform.position = newPosition;
        _rotationSpeed = rotationSpeed;
        
        if (newSprite != null)
        {
            _spriteRenderer.sprite = newSprite;
        }

        _rotateClockwise = Random.Range(0, 1) == 0;
    }

    private void Update()
    {
        OrbitTransform.Rotate(Vector3.forward, Time.deltaTime * (_rotateClockwise ? _rotationSpeed : -_rotationSpeed));
    }
}
