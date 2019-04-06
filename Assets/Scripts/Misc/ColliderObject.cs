using System;
using UnityEngine;

public class ColliderObject : MonoBehaviour
{
    [NonSerialized]
    public Action<Collider2D> OnTriggerEnter;
    
    [NonSerialized]
    public Action<Collision2D> OnCollisionEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (OnTriggerEnter != null)
        {
            OnTriggerEnter.Invoke(other);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (OnCollisionEnter != null)
        {
            OnCollisionEnter.Invoke(other);
        }
    }
}
