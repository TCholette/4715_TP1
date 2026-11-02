using UnityEngine;

public class Door : ToggleWithButton
{
    [SerializeField] protected Animator anim;
    [SerializeField] protected Collider2D collision;

    protected override void Open()
    {
        anim.SetTrigger("Open");
    }

    protected override void Close()
    {
        anim.SetTrigger("Close");
        collision.enabled = true;
    }

    public virtual void EndOpen()
    {
        collision.enabled = false;
    }
}
