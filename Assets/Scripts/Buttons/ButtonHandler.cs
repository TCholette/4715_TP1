using System;
using System.Collections;
using UnityEditor.MemoryProfiler;
using UnityEngine;


public enum ButtonMode
{
    Base,
    Timer,
}
public class ButtonHandler: MonoBehaviour 
{
    [SerializeField] private ToggleWithButton target;
    [SerializeField] private Animator anim;
    [SerializeField] private LineRenderer line;
    [SerializeField] private LineRenderer timer;
    [SerializeField] private Transform connection;
    [SerializeField] private ButtonMode buttonMode;
    [SerializeField] private float closeTimer;
    [SerializeField] private bool startsEnabled = false;
    private bool _isEnabled;
    private bool _isReady = true;

    private void Awake()
    {
        line.SetPosition(0, connection.position);
        line.SetPosition(1, target.connection.position);
        line.startColor = Color.red;
        line.endColor = Color.red;
        if (startsEnabled)
        {
            Activate();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_isReady)
            {
                _isReady = false;
                if (!_isEnabled)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
        }
    }

    private void Activate()
    {
        line.startColor = Color.green;
        line.endColor = Color.green;
        target.Activate();
        anim.SetTrigger("Click");
        _isEnabled = true;
        if (buttonMode == ButtonMode.Timer && !startsEnabled)
        {
            StartCoroutine(Timer());
        }
    }

    private void Deactivate()
    {
        line.startColor = Color.red;
        line.endColor = Color.red;
        target.Deactivate();
        anim.SetTrigger("Release");
        _isEnabled = false;
        if (buttonMode == ButtonMode.Timer && startsEnabled)
        {
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        float time = 0;
        timer.gameObject.SetActive(true);
        while (time < closeTimer)
        {
            time += Time.deltaTime;

            float maxDelta = 1;
            float delta = Mathf.Lerp(maxDelta, 0, time/closeTimer);

            timer.SetPosition(0, new Vector3(transform.position.x - delta, timer.transform.position.y, transform.position.z));
            timer.SetPosition(1, new Vector3(transform.position.x + delta, timer.transform.position.y, transform.position.z));
            yield return null;
        }
        timer.gameObject.SetActive(false);
        if (!startsEnabled)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    public void Ready()
    {
        _isReady = true;
    }
}
