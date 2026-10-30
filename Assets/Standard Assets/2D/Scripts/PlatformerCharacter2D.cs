using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SpeedTree.Importer;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;
using UnityEngine.TextCore.Text;

#pragma warning disable 649
namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {

        [SerializeField] private Animator sphereAnim;
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        public float MaxSpeed { get { return m_MaxSpeed; } }
        [SerializeField] public float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        //[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        // [SerializeField] private float m_WallJumpHorizontalForce = 400f;
        // [SerializeField] private int m_MaxJumpCount = 1;

        [SerializeField] public Collider2D m_headCollider;
        [SerializeField] public Collider2D m_feetCollider;
        // [SerializeField] public Collider2D m_wallCollider;


        // [SerializeField] public float m_WallJumpMoveDisableTime;

        [SerializeField] public float m_Deceleration;
        [SerializeField] public float m_AirDeceleration;
        [SerializeField] public float m_Acceleration;

        [SerializeField] public float m_ChargeSpeed;
        [SerializeField] public float m_MaxChargeForce;
        [SerializeField] public GameObject m_ChargeIndicator;

        [SerializeField] public GameObject m_Glider;
        // [SerializeField] public float m_GliderMaxDescentSpeed;

        [SerializeField] public Animator m_Anim;

        [SerializeField] private GameObject _glideSelectorMenu;


        //private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        //const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        [SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
        //private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        //const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Rigidbody2D m_Rigidbody2D;
        public Rigidbody2D Rigidbody { get { return m_Rigidbody2D; } }

        public bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private bool m_IsCrouching = false;

        private float m_MoveDirection = 0f;
       
        public bool m_IsMoveDisabled = false;

        private AbsGlider _gliderMechanic;

        // private int m_JumpCount = 0;
        // private float m_JumpCharge = 0f;
        // private bool m_IsGliding = false;
        // private bool m_IsWallGrabbing = false;
        // private float _wallDirection = 0;
        // private Coroutine m_WallGrabRoutine;

        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            _gliderMechanic = m_Glider.GetComponent<AbsGlider>();
            _gliderMechanic.Init(this);
        }

        /*
        private IEnumerator WallGrabTimerRoutine() {
            m_IsWallGrabbing = true;
            yield return new WaitForSeconds(3);
            m_IsWallGrabbing = false;
        }
        */

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = true;
                m_Anim.SetBool("Ground", m_Grounded);
            }

            /*
            else if (collision.otherCollider == m_wallCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_WallDirection = collision.GetContact(0).point.x - transform.position.x;
                if (m_WallGrabRoutine != null) {
                   StopCoroutine(m_WallGrabRoutine);
                }
                m_WallGrabRoutine = StartCoroutine(WallGrabTimerRoutine());
            }
            */
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = false;
            }

            /*
            else if (collision.otherCollider == m_wallCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_WallDirection = 0;
                m_IsWallGrabbing = false;
            }
            */
        }

        private void FixedUpdate()
        {


            /*
            if (m_IsWallGrabbing) {
                m_wallCollider.sharedMaterial.friction = 50;
                m_Rigidbody2D.linearVelocityX += m_WallDirection;
            } else {
                m_wallCollider.sharedMaterial.friction = 0;
                m_Rigidbody2D.linearVelocityX -= m_WallDirection * 0.25f;
            }
            */

            if(_gliderMechanic.isEnabled) _gliderMechanic.FixedUpdateStrategy();

            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.linearVelocity.y);
            Move(m_MoveDirection);

    

            /*
            if (m_IsGliding && !m_Grounded && m_Rigidbody2D.linearVelocityY < -m_GliderMaxDescentSpeed)
            {
                m_Rigidbody2D.linearVelocityY = Mathf.Lerp(m_Rigidbody2D.linearVelocityY, -m_GliderMaxDescentSpeed, 1 / -m_Rigidbody2D.linearVelocityY);
            }
            */
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

            /*
            if (m_IsWallGrabbing && direction * m_WallDirection > 0) {
                m_wallCollider.sharedMaterial.friction = 0;
                m_Rigidbody2D.linearVelocityY = Mathf.Abs(direction) * m_MaxSpeed * 0.5f;
                m_HandAnim.SetTrigger("isClimbing");
            }
            else {
                m_HandAnim.SetTrigger("isIdle");
            }
            */

            m_Anim.SetFloat("Speed", Mathf.Abs(direction));
            if (direction != 0 && (m_Grounded || m_AirControl))
            {
                sphereAnim.SetBool("isWalking", true);
                if (m_Grounded)
                {
                    float targetVelocityX = direction * m_MaxSpeed * (m_IsCrouching? m_CrouchSpeed : 1);
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

        /*
        private IEnumerator WallJumpTimer()
        {
            m_IsMoveDisabled = true;
            yield return new WaitForSeconds(m_WallJumpMoveDisableTime);
            m_IsMoveDisabled = false;
        }
        */

        /*
        public void OnCrouch(InputAction.CallbackContext input)
        {
            if (input.performed)
            {
                m_IsCrouching = true;
            }
            if (input.canceled)
            {
                m_IsCrouching = false;
            }
            m_Anim.SetBool("Crouch", m_IsCrouching);
        }
        */
        public void OnJump(InputAction.CallbackContext input)
        {
            if (input.performed)
            {

                if (m_Grounded)
                {
                    if (m_IsCrouching)
                    {
                        // StartCoroutine(ChargeJump());
                    }
                    else
                    {
                        // m_JumpCount = 0;
                        m_Grounded = false;
                        m_Anim.SetBool("Ground", false);
                        m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpForce);
                    }
                }

                if (_gliderMechanic.isEnabled) _gliderMechanic.JumpStrategy();

                /*
                else if (m_WallDirection != 0)
                {
                    m_JumpCount = 0;
                    if (m_WallDirection > 0)
                    {
                        Vector3 theScale = transform.localScale;
                        theScale.x = -Mathf.Abs(theScale.x);
                        transform.localScale = theScale;
                        m_FacingRight = false;
                        m_Rigidbody2D.linearVelocity = new Vector2(-m_WallJumpHorizontalForce, m_JumpForce);
                    }
                    else
                    {
                        Vector3 theScale = transform.localScale;
                        theScale.x = Mathf.Abs(theScale.x);
                        transform.localScale = theScale;
                        m_FacingRight = true;
                        m_Rigidbody2D.linearVelocity = new Vector2(m_WallJumpHorizontalForce, m_JumpForce);
                    }
                    StartCoroutine(WallJumpTimer());
                    m_WallDirection = 0;
                    m_Anim.SetBool("Ground", false);
                }
                */

                /*
                else if (m_MaxJumpCount > m_JumpCount) // Double-Jump
                {
                    m_JumpCount++;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpForce);
                }
                */
            }
            /*
            if (input.canceled)
            {
                if (m_Grounded && m_JumpCharge != 0)
                {
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpCharge);
                }
            }
            */
        }

        /*
        private IEnumerator ChargeJump()
        {
            m_JumpCount = 0;
            m_JumpCharge = m_JumpForce;
            while (m_IsCrouching && m_Grounded)
            {
                if (m_ChargeSpeed == 0)
                {
                    yield break;
                }
                yield return new WaitForSeconds(1/m_ChargeSpeed);
                m_JumpCharge += 1;
                m_JumpCharge = Mathf.Min(m_MaxChargeForce, m_JumpCharge);
                m_ChargeIndicator.transform.localScale = new Vector3(m_ChargeIndicator.transform.localScale.x, (m_JumpCharge - m_JumpForce) / m_MaxChargeForce);
                m_ChargeIndicator.transform.localPosition = new Vector3(m_ChargeIndicator.transform.localPosition.x, (m_JumpCharge - m_JumpForce) / (2 * m_MaxChargeForce));
            }
            m_JumpCharge = 0f;
            m_ChargeIndicator.transform.localScale = new Vector3(m_ChargeIndicator.transform.localScale.x, 0);
        }
        */

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        private void OpenGlider()
        {
            AbsGlider gliderMechanic = m_Glider.GetComponent<AbsGlider>();
            if(_gliderMechanic != gliderMechanic) {
                if(_gliderMechanic != null) _gliderMechanic.isEnabled = false;
                _gliderMechanic = gliderMechanic;
                _gliderMechanic.Init(this);
            }
            m_Glider.SetActive(true);
            _gliderMechanic.isGliding = true;
            _gliderMechanic.isEnabled = true;
        }

        private void CloseGlider() {
            m_Glider.SetActive(false);
            _gliderMechanic.isGliding = false;
            _gliderMechanic.isEnabled = false;
        }

        public void OnGlide(InputAction.CallbackContext input)
        {
            if (input.performed) {
                if (!m_Glider.activeSelf) {
                    OpenGlider();
                }
                else {
                    CloseGlider();
                }
            }
           
        }

        public void OnGlideSwitch(InputAction.CallbackContext input) {
            if (input.performed) {
                _glideSelectorMenu.SetActive(true);
            } else if (input.canceled) {
                _glideSelectorMenu.SetActive(false);
            }
        }

        public void SwitchGlider(GameObject glider) {
            bool wasActive = m_Glider.activeSelf;
            m_Glider.SetActive(false);
            m_Glider = glider;
            if (wasActive) OpenGlider();
        }
    }
}
