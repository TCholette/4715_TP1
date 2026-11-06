using System;
using Unity.Properties;
using UnityEngine;

public abstract class AbsGlider : MonoBehaviour
{
    [HideInInspector] public bool isEnabled = false;
    [HideInInspector] public bool isGliding = false;
    public float jumpForceMultiplier = 1f;
    protected static float _jumpCount = 0;

    protected PlatformerCharacter2D _character;

    [SerializeField] private float stabilisationSpeed = 2f;
    [SerializeField] protected float _gliderMaxDescentSpeed = 100;
    public void Init(PlatformerCharacter2D character) {
        if (_character == null) {
            _character = character;
        }
    }

    protected void ResetJumps() {
        if ((_character.GetComponent<Rubberband>().isInRubberband || _character.IsGrounded)) {
            _jumpCount = 0;
        }
    }

    protected void TryGlide() {
        if (isGliding && !_character.IsGrounded && _character.Rigidbody.linearVelocityY < -_gliderMaxDescentSpeed) {
            _character.Rigidbody.linearVelocityY = Mathf.Lerp(_character.Rigidbody.linearVelocityY, -_gliderMaxDescentSpeed, stabilisationSpeed / -_character.Rigidbody.linearVelocityY);
        }
    }

    abstract public void FixedUpdateStrategy();

    abstract public void MoveStrategy(float direction);

    abstract public void JumpStrategy();

    abstract public bool PerformAirDirectionCheckStrategy();
}
