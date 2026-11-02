using UnityEngine;

public class SequenceDoor : Door
{
    [SerializeField] private SequenceDoor next;

    public override void EndOpen()
    {
        collision.enabled = false;
        if (next != null)
        {
            next.Activate();
        }

    }
    public void EndClose()
    {
        collision.enabled = true;
        if (next != null)
        {
            next.Deactivate();
        }
    }
}
