using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlatformerCharacter2D : MonoBehaviour
{

    [SerializeField] private Animator sphereAnim;
    [SerializeField] private float m_MaxSpeed = 10f;
    public float MaxSpeed { get { return m_MaxSpeed; } }
    [SerializeField] public float m_JumpForce = 400f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] public Collider2D m_headCollider;
    [SerializeField] public Collider2D m_feetCollider;
    [SerializeField] public float m_Deceleration;
    [SerializeField] public float m_AirDeceleration;
    [SerializeField] public float m_Acceleration;

    [SerializeField] private GameObject m_Glider;
    [SerializeField] private bool m_Grounded;
    private Rigidbody2D m_Rigidbody2D;
    public Rigidbody2D Rigidbody { get { return m_Rigidbody2D; } }

    public bool m_FacingRight = true;

    private bool m_IsJumping = false;

    private float m_MoveDirection = 0f;

    public bool m_IsMoveDisabled = false;

    private AbsGlider _gliderMechanic;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _gliderMechanic = m_Glider.GetComponent<AbsGlider>();
        _gliderMechanic.Init(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
        {
            m_Grounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
        {
            m_Grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
        {
            m_Grounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (_gliderMechanic.isEnabled) _gliderMechanic.FixedUpdateStrategy();

        Move(m_MoveDirection);
        Jump();
    }

    public bool IsGrounded
    {
        get => m_Grounded;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        m_MoveDirection = input.ReadValue<Vector2>().x;
    }
    public void Move(float direction)
    {
        if (m_IsMoveDisabled)
        {
            return;
        }

        if (_gliderMechanic.isEnabled) _gliderMechanic.MoveStrategy(direction);

        if (direction != 0 && (m_Grounded || m_AirControl))
        {
            sphereAnim.SetBool("isWalking", true);
            if (m_Grounded)
            {
                float targetVelocityX = direction * m_MaxSpeed;
                m_Rigidbody2D.linearVelocityX = Mathf.MoveTowards(m_Rigidbody2D.linearVelocityX, targetVelocityX, m_Acceleration * Time.deltaTime);
            }
            else
            {
                float targetVelocityX = direction * m_MaxSpeed;
                m_Rigidbody2D.linearVelocityX = Mathf.MoveTowards(m_Rigidbody2D.linearVelocityX, targetVelocityX, m_Acceleration * Time.deltaTime);
            }

            if ((direction < 0) == m_FacingRight)
            {
                Flip();
            }
        }
        else if (direction == 0 && !m_Grounded && _gliderMechanic.PerformAirDirectionCheckStrategy())
        {
            sphereAnim.SetBool("isWalking", false);
            m_Rigidbody2D.linearVelocityX *= (float)Math.Exp(-m_AirDeceleration * Time.deltaTime);

            if (Mathf.Abs(m_Rigidbody2D.linearVelocityX) < 0.05f)
            {
                m_Rigidbody2D.linearVelocityX = 0f;
            }
        }
        else if (direction == 0 && m_Grounded)
        {
            sphereAnim.SetBool("isWalking", false);
            m_Rigidbody2D.linearVelocityX *= (float)Math.Exp(-m_Deceleration * Time.deltaTime);

            if (Mathf.Abs(m_Rigidbody2D.linearVelocityX) < 0.05f)
            {
                m_Rigidbody2D.linearVelocityX = 0f;
            }
        }
    }
    private void Jump()
    {
        if (m_IsJumping)
        {
            if (m_Grounded)
            {
                float gliderMult = _gliderMechanic.isEnabled ? _gliderMechanic.jumpForceMultiplier : 1f;
                m_Rigidbody2D.linearVelocityY = m_JumpForce * gliderMult;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            m_IsJumping = true;
            if (_gliderMechanic.isEnabled) _gliderMechanic.JumpStrategy();
        }
        else if (input.canceled)
        {
            m_IsJumping = false;
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OpenGlider()
    {
        AbsGlider gliderMechanic = m_Glider.GetComponent<AbsGlider>();
        if (_gliderMechanic != gliderMechanic)
        {
            if (_gliderMechanic != null) _gliderMechanic.isEnabled = false;
            _gliderMechanic = gliderMechanic;
            _gliderMechanic.Init(this);
        }
        m_Glider.SetActive(true);
        _gliderMechanic.isGliding = true;
        _gliderMechanic.isEnabled = true;
    }

    private void CloseGlider()
    {
        m_Glider.SetActive(false);
        _gliderMechanic.isGliding = false;
        _gliderMechanic.isEnabled = false;
    }

    public void OnGlide(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!m_Glider.activeSelf)
            {
                OpenGlider();
            }
            else
            {
                CloseGlider();
            }
        }
    }

    public void SwitchGlider(GameObject glider)
    {
        bool wasActive = m_Glider.activeSelf;
        m_Glider.SetActive(false);
        m_Glider = glider;
        if (wasActive) OpenGlider();
    }
}
