using UnityEngine;

public class SequenceDoor : Door
{
    [SerializeField] private SequenceDoor next;

    public override void EndOpen()
    {
        if (collision != null)
        {
            collision.enabled = false;
        }
        if (next != null)
        {
            next.Open();
        }

    }
    public void EndClose()
    {
        if (collision != null)
        {
            collision.enabled = true;
        }
        if (next != null)
        {
            next.Close();
        }
    }
}
