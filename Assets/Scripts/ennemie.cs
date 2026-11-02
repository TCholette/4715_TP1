using System;
using UnityEngine;

public class ennemie : MonoBehaviour
{
    private Collider2D collider;
    void Awake()
    {
        this.collider = this.GetComponent<Collider2D>();
        if (collider == null)
        {
            throw new ArgumentNullException(nameof(collider));
        }
    }

    //void Update() { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: " + collision.gameObject.name + "; " + collision.gameObject.layer);
        if (collision.gameObject.layer == 0)
        {
            collision.gameObject.transform.position = CheckPointManager2.currentActiveCheckPoint.gameObject.transform.position;
        }
    }
}
