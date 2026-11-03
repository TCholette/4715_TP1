using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GliderHands:AbsGlider
{

    [SerializeField] private float _wallDurationSeconds = 0;
    [SerializeField] private float _wallSpeedMultiplier = 0.25f;
    [SerializeField] private float _wallJumpHorizontalForce = 400f;
    [SerializeField] public float m_WallJumpMoveDisableTime;
    [SerializeField] public float m_WallGrabRegenCooldownTime = 0.5f;
    [SerializeField] public Animator _handAnim;
    [SerializeField] private Collider2D _collider;


    private bool _isWallGrabbing = false;
    private Coroutine _wallGrabRoutine;
    private float _wallDirection = 0;

    private float _wallTime = 0;

    private bool _canWallGrab = false;

    public override void FixedUpdateStrategy() {
        if (_character.IsGrounded) {
            _character.m_IsMoveDisabled = false;
            _canWallGrab = true;
        }
        if (_isWallGrabbing) {
            _collider.sharedMaterial.friction = 50;
            _character.Rigidbody.linearVelocityX += _wallDirection * 0.25f;
        }
        else {
            _collider.sharedMaterial.friction = 0;
            _character.Rigidbody.linearVelocityX -= _wallDirection * 0.25f;
        }

        TryGlide();
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if(!isEnabled) return; 
        if (collision.gameObject.CompareTag("Ground")) {
            if (_canWallGrab) _wallTime = 0;
            _wallDirection = collision.GetContact(0).point.x - transform.position.x;
            if (_wallGrabRoutine != null) {
                StopCoroutine(_wallGrabRoutine);
            }
            _wallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
        }
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (!isEnabled) return;
        if (collision.gameObject.CompareTag("Ground")) {
            if (!_isWallGrabbing && _character.IsGrounded) {
                if (_wallGrabRoutine != null) {
                    StopCoroutine(_wallGrabRoutine);
                }
                _wallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
            }

        }
    }

    public void OnCollisionExit2D(Collision2D collision) {
        if (!isEnabled) return;
        if (collision.gameObject.CompareTag("Ground")) {
            _wallDirection = 0;
            StartCoroutine(WallGrabStopRoutine());
            _isWallGrabbing = false;
        }
    }

    public override void MoveStrategy(float direction) {
        if (_isWallGrabbing && direction * _wallDirection > 0) {
            _collider.sharedMaterial.friction = 0;
            _character.Rigidbody.linearVelocityY = Mathf.Abs(direction) * _character.MaxSpeed * _wallSpeedMultiplier;
            _character.Rigidbody.linearVelocityX = 0;
            _handAnim.SetTrigger("isClimbing");
        }
        else {
            _handAnim.SetTrigger("isIdle");
        }
    }

    public override void JumpStrategy() {
        if (_wallDirection != 0) {
            if (_wallDirection > 0) {
                Vector3 theScale = _character.transform.localScale;
                theScale.x = -Mathf.Abs(theScale.x);
                _character.transform.localScale = theScale;
                _character.m_FacingRight = false;
                _character.Rigidbody.linearVelocity = new Vector2(-_wallJumpHorizontalForce, _character.m_JumpForce);
            }
            else {
                Vector3 theScale = _character.transform.localScale;
                theScale.x = Mathf.Abs(theScale.x);
                _character.transform.localScale = theScale;
                _character.m_FacingRight = true;
                _character.Rigidbody.linearVelocity = new Vector2(_wallJumpHorizontalForce, _character.m_JumpForce);
            }
            StartCoroutine(WallJumpTimer());
            _wallDirection = 0;
            _character.m_Anim.SetBool("Ground", false);
        }
    }

    public override bool PerformAirDirectionCheckStrategy() {
        if (!isEnabled) return true;
        return !isGliding && !_isWallGrabbing;
    }

    private IEnumerator WallGrabTimerRoutine() {
        _isWallGrabbing = true;
        while (_wallTime < _wallDurationSeconds) {
            _wallTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        _isWallGrabbing = false;
        _character.m_IsMoveDisabled = true;
        float timeOutDuration = _wallDurationSeconds * (1f / (_character.MaxSpeed * _wallSpeedMultiplier) + m_WallJumpMoveDisableTime);
        yield return new WaitForSeconds(timeOutDuration);
        _character.m_IsMoveDisabled = false;
    }

    private IEnumerator WallGrabStopRoutine() {
        _canWallGrab = false;
        yield return new WaitForSeconds(m_WallGrabRegenCooldownTime);
        _canWallGrab = true;
    }

    private IEnumerator WallJumpTimer() {
        _character.m_IsMoveDisabled = true;
        yield return new WaitForSeconds(m_WallJumpMoveDisableTime);
        _character.m_IsMoveDisabled = false;
    }
}
