using UnityEngine;

public class Rubberband : MonoBehaviour
{
    [SerializeField] private Vector2 sendDirection = Vector2.up;
    [SerializeField] private float rubberbandForce;

    public void OnTriggerEnter2D(Collider2D col)
    {
        var rigid = col.GetComponent<Rigidbody2D>();
        if (rigid == null) return;
        // rigid.AddForce(sendDirection.normalized * rubberbandForce, ForceMode2D.Impulse);
    }
}
