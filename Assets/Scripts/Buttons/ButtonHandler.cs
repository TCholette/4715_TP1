using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum ButtonMode
{
    Base,
    Timer,
}
public class ButtonHandler: MonoBehaviour 
{
    [SerializeField] private List<ToggleWithButton> targets;
    private List<LineRenderer> _allLines = new();
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject lineTemplate;
    [SerializeField] private LineRenderer timer;
    [SerializeField] private Transform connection;
    [SerializeField] private ButtonMode buttonMode;
    [SerializeField] private float closeTimer;
    [SerializeField] private bool startsEnabled = false;
    private bool _isEnabled;
    private bool _isReady = true;

    private void Awake()
    {
        foreach (ToggleWithButton target in targets)
        {
            LineRenderer line = Instantiate(lineTemplate, transform).GetComponent<LineRenderer>();
            line.SetPosition(0, connection.position);
            line.SetPosition(1, target.connection.position);
            line.startColor = Color.red;
            line.endColor = Color.red;
            target.Setup(this);
            _allLines.Add(line);
        }
        if (startsEnabled)
        {
            Activate();
        }
        else
        {
            Deactivate();
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

    public void Activate(bool isFirst = true)
    {
        foreach (LineRenderer line in _allLines)
        {
            line.startColor = Color.green;
            line.endColor = Color.green;
        }
        anim.SetBool("isClicking", true);
        _isEnabled = true;
        if (isFirst)
        {
            foreach (ToggleWithButton target in targets)
            {
                target.Activate();
            }
            if (buttonMode == ButtonMode.Timer && !startsEnabled)
            {
                StartCoroutine(Timer());
            }
        }
    }

    public void Deactivate(bool isFirst = true)
    {
        foreach (LineRenderer line in _allLines)
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
        anim.SetBool("isClicking", false);
        _isEnabled = false;
        if (isFirst)
        {
            foreach (ToggleWithButton target in targets)
            {
                target.Deactivate();
            }
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
