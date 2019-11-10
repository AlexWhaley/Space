using UnityEngine;

public class SatelliteController : ObstacleController
{
    public override void SetColour(ObstacleColour colour)
    {
        base.SetColour(colour);
        _spriteRenderer.sprite = SpriteManager.Instance.GetRandomSatelliteSprite(_colour);
    }

    protected override void DestroyObstacle(Vector2 playerTrajectory)
    {
        //var asteroidFragments = ObjectPoolManager.Instance.AsteroidFragmentsObjectPool.Get();
        //asteroidFragments.Controller.FireFragments(transform, _rigidBody.velocity * 0.9f, playerTrajectory, _colour);
    }

    public void ParentAndSetPosition(Vector2 localPosition, Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = localPosition;
    }
}
