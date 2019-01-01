using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [Header("Big Planet Sprites")]
    [SerializeField]
    private List<Sprite> _bigRedPlanetSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _bigBluePlanetSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _bigGreenPlanetSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _bigOrangePlanetSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _bigPinkPlanetSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _bigPurplePlanetSprites = new List<Sprite>(3);
    
    [Header("Asteroid Sprites")]
    [SerializeField]
    private List<Sprite> _blueAsteroidSprites = new List<Sprite>(3);
    [SerializeField]
    private List<Sprite> _yellowAsteroidSprites = new List<Sprite>(3);

    private enum PlanetColours
    {
        Red,
        Blue,
        Green,
        Orange,
        Pink,
        Purple
    }

    private List<PlanetColours> _randomPlanetColourList = new List<PlanetColours>
    {
        PlanetColours.Blue,
        PlanetColours.Green,
        PlanetColours.Orange,
        PlanetColours.Pink,
        PlanetColours.Purple
    };

    private PlanetColours _lastRandomPlanetColour = PlanetColours.Red;

    private static SpriteManager _instance;
    public static SpriteManager Instance
    {
        get
        {
            return _instance;
        }
    }

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

    public Sprite GetRandomPlanetSprite()
    {
        int randomColourIndex = Random.Range(0, _randomPlanetColourList.Count);

        var randomColour = _randomPlanetColourList[randomColourIndex];
        _randomPlanetColourList.RemoveAt(randomColourIndex);

        _randomPlanetColourList.Add(_lastRandomPlanetColour);
        _lastRandomPlanetColour = randomColour;

        switch (randomColour)
        {
            case PlanetColours.Red:
                return _bigRedPlanetSprites[Random.Range(0, _bigRedPlanetSprites.Count)];
            case PlanetColours.Blue:
                return _bigBluePlanetSprites[Random.Range(0, _bigBluePlanetSprites.Count)];
            case PlanetColours.Green:
                return _bigGreenPlanetSprites[Random.Range(0, _bigGreenPlanetSprites.Count)];
            case PlanetColours.Orange:
                return _bigOrangePlanetSprites[Random.Range(0, _bigOrangePlanetSprites.Count)];
            case PlanetColours.Pink:
                return _bigPinkPlanetSprites[Random.Range(0, _bigPinkPlanetSprites.Count)];
            case PlanetColours.Purple:
                return _bigPurplePlanetSprites[Random.Range(0, _bigPurplePlanetSprites.Count)];
            default:
                return null;
        }
    }
}
