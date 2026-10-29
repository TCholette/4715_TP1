using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GliderHands:MonoBehaviour
{

    [SerializeField] private float _wallDurationSeconds = 0;
    [SerializeField] private float _wallSpeedMultiplier = 0.25f;
    [SerializeField] private float _gliderMaxDescentSpeed = 100;
    [SerializeField] private float _wallJumpHorizontalForce = 400f;
    [SerializeField] public float m_WallJumpMoveDisableTime;
    [SerializeField] public Animator _handAnim;
    [SerializeField] private Collider2D _collider;

    public bool isGliding = false;

    private bool _isWallGrabbing = false;
    private Coroutine _wallGrabRoutine;
    private float _wallDirection = 0;

    private PlatformerCharacter2D _character;

    public void Init(PlatformerCharacter2D character) {
        _character = character;
    }

    public void FixedUpdateStrategy() {
        if (_isWallGrabbing) {
            _collider.sharedMaterial.friction = 50;
            _character.Rigidbody.linearVelocityX += _wallDirection;
        }
        else {
            _collider.sharedMaterial.friction = 0;
            _character.Rigidbody.linearVelocityX -= _wallDirection * 0.25f;
        }

        if (isGliding && !_character.IsGrounded && _character.Rigidbody.linearVelocityY < -_gliderMaxDescentSpeed) {
            _character.Rigidbody.linearVelocityY = Mathf.Lerp(_character.Rigidbody.linearVelocityY, -_gliderMaxDescentSpeed, 1 / -_character.Rigidbody.linearVelocityY);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            _wallDirection = collision.GetContact(0).point.x - transform.position.x;
            if (_wallGrabRoutine != null) {
                StopCoroutine(_wallGrabRoutine);
            }
            _wallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
        }
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            if (!_isWallGrabbing && _character.IsGrounded) {
                if (_wallGrabRoutine != null) {
                    StopCoroutine(_wallGrabRoutine);
                }
                _wallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
            }

        }
    }

    public void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            _wallDirection = 0;
            _isWallGrabbing = false;
        }
    }

    public void MoveStrategy(float direction) {
        if (_isWallGrabbing && direction * _wallDirection > 0) {
            _collider.sharedMaterial.friction = 0;
            _character.Rigidbody.linearVelocityY = Mathf.Abs(direction) * _character.MaxSpeed * _wallSpeedMultiplier;
            _handAnim.SetTrigger("isClimbing");
        }
        else {
            _handAnim.SetTrigger("isIdle");
        }
    }

    public void JumpStrategy() {
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

    public bool PerformAirDirectionCheckStrategy() {
        return !isGliding && !_isWallGrabbing;
    }

    private IEnumerator WallGrabTimerRoutine() {
        _isWallGrabbing = true;
        yield return new WaitForSeconds(_wallDurationSeconds);
        _isWallGrabbing = false;
    }

    private IEnumerator WallJumpTimer() {
        _character.m_IsMoveDisabled = true;
        yield return new WaitForSeconds(m_WallJumpMoveDisableTime);
        _character.m_IsMoveDisabled = false;
    }
}
