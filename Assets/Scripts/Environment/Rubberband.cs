using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Rubberband : MonoBehaviour
{
    [SerializeField] private Vector2 sendDirection = Vector2.up;
    [SerializeField] private float rubberbandForce = 50;
    [HideInInspector] public bool isInRubberband = false;
    private bool jumped = false;
    [SerializeField] private float maxTimeSec = 1;
    [SerializeField] private float minJumpMod = 0.5f;
    [SerializeField] private float horizontalExtraForce = 1.5f;

    private bool _isJumping = false;

    private Coroutine stretchRoutine;

    [SerializeField] private LineRenderer timer;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("rubberband"))
        {
            if (_isJumping)
            {
                if (stretchRoutine != null)
                {
                    StopCoroutine(stretchRoutine);
                }
                StartCoroutine(Timer());
            }
            isInRubberband = true;
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
            isInRubberband = false;
        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        //if (input.ReadValue<Vector2>().magnitude > 0)
        //{
        //    sendDirection = input.ReadValue<Vector2>();
        //}
    }

    public void OnJump(InputAction.CallbackContext input)
    {

        if (input.performed)
        {
            _isJumping = true;
            if (isInRubberband)
            {
                if (stretchRoutine != null)
                {
                    StopCoroutine(stretchRoutine);
                }
                StartCoroutine(Timer());
            }
        }

        if (input.canceled)
        {
            _isJumping = false;
            if (isInRubberband)
            {
                if (stretchRoutine != null)
                {
                    StopCoroutine(stretchRoutine);
                }

                timer.gameObject.SetActive(false);

                var durationRatio = (float)input.duration / maxTimeSec;
                var forceMod = Mathf.Clamp(durationRatio, minJumpMod, 1);
                var rigid = GetComponent<Rigidbody2D>();

                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                Vector3 sendDirection = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
                rigid.AddForce(forceMod * rubberbandForce * new Vector3(sendDirection.normalized.x * horizontalExtraForce, sendDirection.normalized.y), ForceMode2D.Impulse);
                jumped = true;
            }
        }
    }


    private IEnumerator Timer()
    {
        float time = 0;
        timer.gameObject.SetActive(true);
        while (time < maxTimeSec)
        {
            time += Time.deltaTime;

            float maxDelta = 1;
            float delta = Mathf.Lerp(0, maxDelta, time / maxTimeSec);

            timer.SetPosition(0, new Vector3(transform.position.x - delta, timer.transform.position.y + 2, transform.position.z));
            timer.SetPosition(1, new Vector3(transform.position.x + delta, timer.transform.position.y + 2, transform.position.z));
            yield return null;
        }
    }
}
