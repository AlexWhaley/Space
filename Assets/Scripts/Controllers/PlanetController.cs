using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetPositionAndSprite(Vector3 newPosition, Sprite newSprite)
    {
        transform.position = newPosition;
        if (newSprite != null)
        {
            _spriteRenderer.sprite = newSprite;
        }
    }
}
