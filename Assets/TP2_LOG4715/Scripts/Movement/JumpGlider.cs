using UnityEngine;

public class JumpGlider : AbsGlider
{
    [SerializeField] private float _maxJumpCount = 0;
    public override void FixedUpdateStrategy() {
        ResetJumps();
        if (isEnabled) {
            TryGlide();
        }
    }

    public override void JumpStrategy() {
        if (isEnabled) {
            if (_maxJumpCount > _jumpCount && !_character.GetComponent<Rubberband>().isInRubberband) {
                _jumpCount++;
                _character.Rigidbody.linearVelocity = new Vector2(_character.Rigidbody.linearVelocityX, _character.m_JumpForce);
            }
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
