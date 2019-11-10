using TMPro;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField] 
    private ShipController _shipController;

    [SerializeField] private float _scoreScale = 0.1f;

    private void Update()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        _scoreText.text = ((int)(_shipController.DistanceTravelled * _scoreScale)).ToString();
    }
}
