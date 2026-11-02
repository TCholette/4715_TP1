using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float originGravityScale = 4f;
    [SerializeField] private Vector2 windDirection = Vector2.up;
    [SerializeField] private float windForce = 40f;

    public void OnTriggerStay2D(Collider2D col)
    {
        var rigid = col.GetComponent<Rigidbody2D>();
        if (rigid == null) return;
        var gliders = rigid.GetComponentsInChildren<AbsGlider>();
        if (gliders.Length == 0) return;
        rigid.AddForce(windDirection.normalized * windForce, ForceMode2D.Force);
    }
}
