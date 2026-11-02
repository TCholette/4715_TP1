using UnityEngine;
using System.Collections.Generic;

public class ToggleWithButton : MonoBehaviour
{
    [SerializeField] public Transform connection;
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
