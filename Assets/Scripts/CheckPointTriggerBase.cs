using System;
using UnityEngine;

public abstract class CheckPointTriggerBase : MonoBehaviour
{
    [SerializeField] protected bool _isStartOfLevel = false;
    [SerializeField] protected bool _isEndOfLevel = false;
    protected bool _isActive = true;


    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite activated;
    [SerializeField] private Sprite deactivated;


    public void Activate()
    {
        sprite.sprite = activated;
    }

    public void Deactivate()
    {
        sprite.sprite = deactivated;
    }
}
