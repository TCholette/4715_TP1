using System;
using UnityEngine;

public class MovingEnemy : EnnemiBase
{
    [SerializeField] protected Vector2[] pointsControles;
    [SerializeField] protected bool replay;
    [SerializeField] protected int cycle = 0;//0 = infinie
    [SerializeField] protected int nextControlPointIndex = 0;
    [SerializeField] protected float vitesse;
    [SerializeField] protected float slowDownDistance = 1f;


    bool rewind = false;
    void Update()
    {
        if (vitesse == 0 || cycle == 0)
            return;
        Vector2 target = pointsControles[nextControlPointIndex];
        Vector2 currentPos = this.transform.position;

        Vector2 dir = target - currentPos;
        float dirMagnitude = dir.magnitude;
        float lerpedSpeed = Mathf.Lerp(0, vitesse, dirMagnitude / slowDownDistance);
        this.transform.position = currentPos + dir.normalized * lerpedSpeed * Time.deltaTime;

        float checkPointTreeshold = 0.25f;
        if (dirMagnitude <= checkPointTreeshold)
        {
            if (replay)
            {
                if (nextControlPointIndex + 1 == pointsControles.Length)
                {
                    rewind = true;
                }
                else if (nextControlPointIndex == 0)
                {
                    rewind = false;
                    if (cycle >= 0)
                    {
                        cycle--;
                        if (cycle <= 0)
                        {
                            cycle = 0;
                        }
                    }
                }
            }

            if (cycle >= 0)
            {
                if (nextControlPointIndex + 1 == pointsControles.Length)
                {
                    cycle--;
                }
                if (cycle <= 0)
                {
                    cycle = 0;
                }
            }

            if (rewind)
            {
                nextControlPointIndex--;
            }
            else
            {
                nextControlPointIndex = (nextControlPointIndex + 1) % pointsControles.Length;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = CheckPointManager2.currentActiveCheckPoint.gameObject.transform.position;
        }
    }
}