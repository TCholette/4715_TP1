using UnityEngine;

public class Door : ToggleWithButton
{
    [SerializeField] protected Animator anim;
    [SerializeField] protected Collider2D collision;

    protected override void Open()
    {
        anim.SetBool("isOpen", true);
    }

    protected override void Close()
    {
        anim.SetBool("isOpen", false);
        collision.enabled = true;
    }

    public virtual void EndOpen()
    {
        collision.enabled = false;
    }
}
