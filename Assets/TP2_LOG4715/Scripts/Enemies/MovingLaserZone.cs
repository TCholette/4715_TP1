using System;
using UnityEngine;

public class MovingLaserZone : MonoBehaviour
{
    [SerializeField] private MovingLaser laser;
    [SerializeField] private Transform start;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (start == null)
            {
                laser.Stop();
            }
            else
            {
                laser.SetZone(start);
            }
        }
    }
}
