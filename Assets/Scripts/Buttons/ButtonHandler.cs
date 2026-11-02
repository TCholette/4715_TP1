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
        target.Setup(this);
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

    public void Activate(bool isFirst = true)
    {
        line.startColor = Color.green;
        line.endColor = Color.green;
        anim.SetTrigger("Click");
        _isEnabled = true;
        if (isFirst)
        {
            target.Activate();
            if (buttonMode == ButtonMode.Timer && !startsEnabled)
            {
                StartCoroutine(Timer());
            }
        }
    }

    public void Deactivate(bool isFirst = true)
    {
        line.startColor = Color.red;
        line.endColor = Color.red;
        anim.SetTrigger("Release");
        _isEnabled = false;
        if (isFirst)
        {
            target.Deactivate();
            if (buttonMode == ButtonMode.Timer && startsEnabled)
            {
                StartCoroutine(Timer());
            }
        }
    }

    private IEnumerator Timer()
    {
        float time = 0;
        timer.gameObject.SetActive(true);
        while (time < closeTimer)
        {
            _isReady = false;
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
