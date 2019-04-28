using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhysicsLayers : MonoBehaviour
{
    public static int Orbit;
    public static int Obstacle;
    public static int PlayerOrbit;
    public static int PlayerObstacle;
    public static int Shield;
    public static int ShieldEnabler;

    private void Awake()
    {
        Orbit = LayerMask.NameToLayer("Orbit");
        Obstacle = LayerMask.NameToLayer("Obstacle");
        PlayerOrbit = LayerMask.NameToLayer("PlayerOrbit");
        PlayerObstacle = LayerMask.NameToLayer("PlayerObstacle");
        Shield = LayerMask.NameToLayer("Shield");
        ShieldEnabler = LayerMask.NameToLayer("ShieldEnabler");
    }
}
