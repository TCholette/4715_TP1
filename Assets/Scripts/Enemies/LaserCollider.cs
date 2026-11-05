using System;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = CheckPointManager2.currentActiveCheckPoint.gameObject.transform.position;
        }
    }
}
