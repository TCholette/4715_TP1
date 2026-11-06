using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [SerializeField] protected Transform[] controlPoints;
    [SerializeField] protected bool replay;
    [SerializeField] protected int cycle = 0;
    [SerializeField] protected int nextControlPointIndex = 0;
    [SerializeField] protected float speed;
    [SerializeField] protected float slowDownDistance = 1f;


    bool rewind = false;
    void Update()
    {
        if (speed == 0 || cycle == 0)
            return;
        Vector2 target = controlPoints[nextControlPointIndex].position;
        Vector2 currentPos = this.transform.position;

        Vector2 dir = target - currentPos;
        float dirMagnitude = dir.magnitude;
        float lerpedSpeed = Mathf.Lerp(0, speed, dirMagnitude / slowDownDistance);
        this.transform.position = currentPos + dir.normalized * lerpedSpeed * Time.deltaTime;

        float checkPointTreeshold = 0.25f;
        if (dirMagnitude <= checkPointTreeshold)
        {
            if (replay)
            {
                if (nextControlPointIndex + 1 == controlPoints.Length)
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
                if (nextControlPointIndex + 1 == controlPoints.Length)
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
                nextControlPointIndex = (nextControlPointIndex + 1) % controlPoints.Length;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = CheckPointManager.currentActiveCheckPoint.gameObject.transform.position;
        }
    }
}