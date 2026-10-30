using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityStandardAssets._2D;

public abstract class AbsGlider : MonoBehaviour
{
    public bool isEnabled = false;
    protected PlatformerCharacter2D _character;
    public bool isGliding = false;
    [SerializeField] protected float _gliderMaxDescentSpeed = 100;
    public void Init(PlatformerCharacter2D character) {
        if (_character == null) {
            _character = character;
        }
    }

    protected void TryGlide() {
        if (isGliding && !_character.IsGrounded && _character.Rigidbody.linearVelocityY < -_gliderMaxDescentSpeed) {
            _character.Rigidbody.linearVelocityY = Mathf.Lerp(_character.Rigidbody.linearVelocityY, -_gliderMaxDescentSpeed, 1 / -_character.Rigidbody.linearVelocityY);
        }
    }

    abstract public void FixedUpdateStrategy();

    abstract public void MoveStrategy(float direction);

    abstract public void JumpStrategy();

    abstract public bool PerformAirDirectionCheckStrategy();
}
