using UnityEngine;

public class RobotLoseZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<RobotRescueHandler>().KillRobots();
        }
    }
}
