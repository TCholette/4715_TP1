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

    [SerializeField] private bool canBeActivated = true;
    [SerializeField] private bool canBeDeactivated = true;

    private void Start()
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
        PlayerEventManager.OnPlayerDeath += ForceDisable;
        if (buttonMode == ButtonMode.Base)
        {
            PlayerEventManager.OnPlayerCheckpoint += (index) => { startsEnabled = _isEnabled; };
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

    private void ForceDisable()
    {
        if (buttonMode == ButtonMode.Timer)
        {
            interruptTimer = true;
        }
        else
        {
            if (startsEnabled)
            {
                if (!_isEnabled)
                {
                    Activate();
                }
            }
            else
            {
                if (_isEnabled)
                {
                    Deactivate();
                }
            }
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
                    if (canBeActivated)
                    {
                        Activate();
                    }
                }
                else
                {
                    if (canBeDeactivated)
                    {
                        Deactivate();
                    }
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
        }
        if (buttonMode == ButtonMode.Timer && !startsEnabled)
        {
            StartCoroutine(Timer());
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
        }
        if (buttonMode == ButtonMode.Timer && startsEnabled)
        {
            StartCoroutine(Timer());
        }
    }

    bool interruptTimer = false;

    private IEnumerator Timer()
    {
        interruptTimer = false;
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
            if (interruptTimer)
            {
                break;
            }
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
