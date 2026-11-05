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
        Open();
    }
    public void Deactivate()
    {
        foreach (ButtonHandler button in allButtons)
        {
            button.Deactivate(false);
        }
        Close();
    }

    protected virtual void Open()
    {
        gameObject.SetActive(isEnabledWhenActive);
    }

    protected virtual void Close()
    {
        gameObject.SetActive(!isEnabledWhenActive);
    }

    public virtual void Setup(ButtonHandler button)
    {
        allButtons.Add(button);
    }
}
