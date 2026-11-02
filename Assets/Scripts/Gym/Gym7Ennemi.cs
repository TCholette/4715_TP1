using System;
using UnityEngine;

public class Gym7Ennemi : EnnemiBase
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: " + collision.gameObject.name + "; " + collision.gameObject.layer);
        if (collision.gameObject.layer == 0)
        {
            collision.gameObject.transform.position = CheckPointManager2.currentActiveCheckPoint.gameObject.transform.position;
        }
    }
}
