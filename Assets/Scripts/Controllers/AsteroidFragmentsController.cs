using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidFragmentsController : MonoBehaviour
{
    private List<AsteroidFragment> _fragments = new List<AsteroidFragment>();
    private Rigidbody2D _rb;
    private Transform _transform;
    [SerializeField]
    private Transform _fragmentsParent;
    [SerializeField]
    private Color _blueFragmentColor;
    [SerializeField]
    private Color _yellowFragmentColor;
    [SerializeField]
    private ParticleSystem _starburstParticleSystem;
    private ParticleSystem[] _particleEffects;

    private void Awake()
    {
        _transform = transform;
        foreach (Transform child in _fragmentsParent)
        {
            _fragments.Add(new AsteroidFragment(child));
        }

        _particleEffects = GetComponentsInChildren<ParticleSystem>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void FireFragments(Transform newTransform, Vector2 currentAsteroidVelocity, Vector2 playerTrajectory, ObstacleColour asteroidColour)
    {
        _rb.velocity = currentAsteroidVelocity;
        _transform.position = newTransform.position;
        _transform.rotation = newTransform.rotation;
        _starburstParticleSystem.startColor = asteroidColour == ObstacleColour.Blue ? _blueFragmentColor : _yellowFragmentColor;

        foreach (var fragment in _fragments)
        {
            fragment.FragmentSprite = SpriteManager.Instance.GetRandomAsteroidFragmentSprite(asteroidColour);
            fragment.Rigidbody.velocity = currentAsteroidVelocity;
            fragment.Rigidbody.AddExplosionForce(20.0f, 5.0f, (Vector2)_transform.position - playerTrajectory.normalized * 1.25f, inverseDistanceModifier: true);

            bool isSpinningClockwise = Random.Range(0, 2) == 0;
            fragment.Rigidbody.angularVelocity = isSpinningClockwise ? Random.Range(25.0f, 50.0f) : Random.Range(-50.0f, -25.0f);
        }
        foreach (var particle in _particleEffects)
        {
            particle.Play();
        }
    }

    public void ResetFragments()
    {
        foreach (var fragment in _fragments)
        {
            fragment.ResetPosition();
        }
        foreach (var particle in _particleEffects)
        {
            particle.Stop();
        }
        
    }
}

public class AsteroidFragment
{
    private Transform _transform;
    private Vector3 _initialLocalPosition;
    private Quaternion _initialRotation;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    
    public AsteroidFragment(Transform fragmentTransform)
    {
        _transform = fragmentTransform;
        _initialLocalPosition = _transform.localPosition;
        _initialRotation = _transform.rotation;
        _rigidbody = _transform.GetComponent<Rigidbody2D>();
        _spriteRenderer = _transform.GetComponent<SpriteRenderer>();
    }

    public Rigidbody2D Rigidbody
    {
        get { return _rigidbody; }
    }

    public Sprite FragmentSprite
    {
        set { _spriteRenderer.sprite = value; }
    }

    public void ResetPosition()
    {
        _transform.localPosition = _initialLocalPosition;
        _transform.rotation = _initialRotation;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0.0f;
    }
}
