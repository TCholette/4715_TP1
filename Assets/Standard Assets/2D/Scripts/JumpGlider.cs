using UnityEngine;

public class JumpGlider : AbsGlider
{
    [SerializeField] private float _maxJumpCount = 0;
    private float _jumpCount = 0;
    public override void FixedUpdateStrategy() {
        if (_character.IsGrounded && _jumpCount != 0) {
            _jumpCount = 0;
        }
        TryGlide();
    }

    public override void JumpStrategy() {
        if (_maxJumpCount > _jumpCount)
        {
            _jumpCount++;
            _character.m_Anim.SetBool("Ground", false);
            _character.Rigidbody.linearVelocity = new Vector2(_character.Rigidbody.linearVelocityX, _character.m_JumpForce);
        }
    }

    public override void MoveStrategy(float direction) {
        return;
    }

    public override bool PerformAirDirectionCheckStrategy() {
        if (!isEnabled) return true;
        return !isGliding;
    }
}
