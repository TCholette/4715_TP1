using UnityEngine;

public abstract class CheckPointTriggerBase : MonoBehaviour
{
    [SerializeField] protected bool _isStartOfLevel = false;
    [SerializeField] protected bool _isEndOfLevel = false;
    protected bool _isActive = true;
}
