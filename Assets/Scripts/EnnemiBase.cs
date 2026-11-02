using System;
using UnityEngine;

public abstract class EnnemiBase : MonoBehaviour
{
    protected Collider2D collider;
    void Awake()
    {
        this.collider = this.GetComponent<Collider2D>();
        if (collider == null)
        {
            throw new ArgumentNullException(nameof(collider));
        }
    }
}
