using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponentInParent<ShipController>().DestroyPlayer();
    }
}
