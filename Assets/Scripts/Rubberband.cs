using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Rubberband : MonoBehaviour
{
    [SerializeField] private Vector2 sendDirection = Vector2.up;
    [SerializeField] private float rubberbandForce = 50;
    private bool inRubberband = false;
    private bool jumped = false;
    private bool startedInside = false;
    [SerializeField] private float maxTimeSec = 1;
    [SerializeField] private float minJumpMod = 0.5f;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("rubberband"))
        {
            inRubberband = true;
            jumped = false;
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if (!jumped && col.gameObject.layer == LayerMask.NameToLayer("rubberband"))
        {
            transform.position = col.transform.position;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("rubberband"))
        {
            inRubberband = false;
            startedInside = false;
        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        sendDirection = input.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (inRubberband && input.started)
        {
            startedInside = true;
        }

        if (inRubberband && startedInside && input.canceled)
        {
            var durationRatio = (float) input.duration / maxTimeSec;
            var forceMod = Mathf.Clamp(durationRatio, minJumpMod, 1);
            var rigid = GetComponent<Rigidbody2D>();
            rigid.AddForce(forceMod * rubberbandForce * sendDirection.normalized, ForceMode2D.Impulse);
            jumped = true;
        }
    }
}
