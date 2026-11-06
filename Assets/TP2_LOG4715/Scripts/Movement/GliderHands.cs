using System.Collections;
using UnityEngine;
public class GliderHands : AbsGlider
{

    [SerializeField] private float _wallDurationSeconds = 0;
    [SerializeField] private float _wallSpeedMultiplier = 0.25f;
    [SerializeField] private float _wallFallMultiplier = 0.25f;
    [SerializeField] private float _wallJumpHorizontalForce = 400f;
    [SerializeField] public float m_WallJumpMoveDisableTime;
    [SerializeField] public float m_WallGrabRegenCooldownTime = 0.5f;
    [SerializeField] public Animator _handAnim;
    [SerializeField] private Collider2D _collider;
    private const float GRAB_GRAVITY_COMPENSATION = 10f;


    private bool _isTryingGrab = false;
    private bool _isWallGrabbing = false;
    private Coroutine _wallGrabRoutine;
    private Coroutine inputDisableRoutine;
    private float _wallDirection = 0;
    private float _lastWallDirection = 0;

    private float _wallTime = 0;

    private bool _canWallGrab = false;

    public override void FixedUpdateStrategy() {
        ResetJumps();
        if (isEnabled) {
            if (_character.IsGrounded) {
                _character.m_IsMoveDisabled = false;
                _canWallGrab = true;
            }
            if (_isWallGrabbing) {
                _character.Rigidbody.linearVelocityX += _wallDirection * 0.25f;
                _character.Rigidbody.AddForceY(GRAB_GRAVITY_COMPENSATION * _character.Rigidbody.gravityScale);
            }
            else {
                _character.Rigidbody.linearVelocityX -= _wallDirection * 0.25f;
            }

            TryGlide();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (isEnabled) {
            _isTryingGrab = true;
            if (collision.gameObject.CompareTag("Ground")) {
                float wallPosition = (collision.GetContact(0).point.x - transform.position.x);
                _wallDirection = wallPosition / Mathf.Abs(wallPosition);
                if (_canWallGrab || _wallDirection != _lastWallDirection) _wallTime = 0;
                if (_wallGrabRoutine != null) {
                    StopCoroutine(_wallGrabRoutine);
                }
                _wallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (isEnabled) {
            if (collision.gameObject.CompareTag("Ground")) {
                if (!_isWallGrabbing && _character.IsGrounded) {
                    if (_wallGrabRoutine != null) {
                        StopCoroutine(_wallGrabRoutine);
                    }
                    _wallGrabRoutine = _character.StartCoroutine(WallGrabTimerRoutine());
                }

            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision) {
        if (isEnabled) {
            _isTryingGrab = false;
            if (collision.gameObject.CompareTag("Ground")) {
                _lastWallDirection = _wallDirection;
                _wallDirection = 0;
                _isWallGrabbing = false;
                _character.StartCoroutine(WallGrabStopRoutine());
            }
        }
    }

    public override void MoveStrategy(float direction) {
        if (isEnabled) {
            if (_isWallGrabbing && direction * _wallDirection > 0) {
                _collider.sharedMaterial.friction = 0f;
                _character.Rigidbody.linearVelocityY = Mathf.Abs(direction) * _character.MaxSpeed * _wallSpeedMultiplier;
                _character.Rigidbody.linearVelocityX = 0;
                _handAnim.SetTrigger("isClimbing");
            }
            else if (_isTryingGrab) {
                _character.Rigidbody.linearVelocityY = Mathf.Abs(direction) * _character.MaxSpeed * _wallFallMultiplier;
                _handAnim.SetTrigger("isGrabbing");
            }
            else {
                _handAnim.SetTrigger("isIdle");
            }
        }
    }


    public override void JumpStrategy() {
        if (isEnabled) {
            if (_wallDirection != 0) {
                if (inputDisableRoutine != null) {
                    _character.StopCoroutine(inputDisableRoutine);
                }
                _character.m_IsMoveDisabled = true;
                inputDisableRoutine = _character.StartCoroutine(WallJumpTimer());
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
            }
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
    }

    private IEnumerator WallGrabStopRoutine()
    {
        _canWallGrab = false;
        yield return new WaitForSeconds(m_WallGrabRegenCooldownTime);
        _canWallGrab = true;
    }

    private IEnumerator WallJumpTimer() {
        yield return new WaitForSeconds(m_WallJumpMoveDisableTime);
        _character.m_IsMoveDisabled = false;
    }
}
