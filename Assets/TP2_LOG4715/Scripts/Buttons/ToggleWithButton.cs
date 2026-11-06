using UnityEngine;
using System.Collections.Generic;

public class ToggleWithButton : MonoBehaviour
{
    [SerializeField] public Transform connection;
    [SerializeField] public bool isEnabledWhenActive;

    private List<ButtonHandler> allButtons = new();
    public void Activate()
    {
        foreach (ButtonHandler button in allButtons)
        {
            button.Activate(false);
        }
        if (!isEnabledWhenActive)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public void Deactivate()
    {
        foreach (ButtonHandler button in allButtons)
        {
            button.Deactivate(false);
        }
        if (!isEnabledWhenActive)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    protected virtual void Open()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Close()
    {
        gameObject.SetActive(true);
    }

    public virtual void Setup(ButtonHandler button)
    {
        allButtons.Add(button);
    }
}
