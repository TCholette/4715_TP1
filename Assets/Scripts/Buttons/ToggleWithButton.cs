using UnityEngine;

public class ToggleWithButton : MonoBehaviour
{
    [SerializeField] public Transform connection;
    public virtual void Activate()
    {
        gameObject.SetActive(false);
    }
    public virtual void Deactivate()
    {
        gameObject.SetActive(true);
    }
}
