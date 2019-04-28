using UnityEngine;

public static class Rigidbody2DExtensions 
{
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, float explosionRadius, Vector2 explosionPosition, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Impulse, bool inverseDistanceModifier = false) 
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0.0f)
        {
            explosionDir /= explosionDistance;
        }
        else 
        {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        if (explosionRadius != 0)
        {
            float distanceModifier = 1 - explosionDistance / explosionRadius;
            distanceModifier = inverseDistanceModifier ? 1 - distanceModifier : distanceModifier;
            
            rb.AddForce(Mathf.Lerp(0, explosionForce, distanceModifier) * explosionDir, mode);
        }
    }
}