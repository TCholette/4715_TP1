using System.Runtime.CompilerServices;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float originGravityScale = 4f;
    [SerializeField] private Vector2 windDirection = Vector2.up;
    [SerializeField] private float startWindForce = 40f;
    [SerializeField] private float windForce = 40f;
    [SerializeField] private float gliderMultiplier = 2;

    public void OnCollisionEnter2D(Collision2D col)
    {
        var rigid = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            return;
        }

        var gliders = rigid.GetComponentsInChildren<AbsGlider>();
        if (gliders.Length != 0)
        {
            rigid.AddForce(windDirection.normalized * gliderMultiplier * startWindForce, ForceMode2D.Force);
        }
        else
        {
            rigid.AddForce(windDirection.normalized * startWindForce, ForceMode2D.Force);
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        var rigid = col.GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            return;
        }

        var gliders = rigid.GetComponentsInChildren<AbsGlider>();
        if (gliders.Length != 0)
        {
            rigid.AddForce(windDirection.normalized * gliderMultiplier * windForce, ForceMode2D.Force);
        }
        else
        {
            rigid.AddForce(windDirection.normalized * windForce / 2, ForceMode2D.Force);
        }
    }
}
