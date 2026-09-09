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
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        //[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        [SerializeField] private float m_WallJumpHorizontalForce = 400f;
        [SerializeField] private int m_MaxJumpCount = 1;

        [SerializeField] public Collider2D m_headCollider;
        [SerializeField] public Collider2D m_feetCollider;
        [SerializeField] public Collider2D m_wallCollider;


        [SerializeField] public float m_WallJumpMoveDisableTime;

        [SerializeField] public float m_Deceleration;
        [SerializeField] public float m_AirDeceleration;
        [SerializeField] public float m_Acceleration;

        [SerializeField] public float m_ChargeSpeed;
        [SerializeField] public float m_MaxChargeForce;
        [SerializeField] public GameObject m_ChargeIndicator;

        //private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        //const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        //private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        //const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private int m_JumpCount = 0;

        private bool m_IsCrouching = false;

        private float m_MoveDirection = 0f;

        private float m_WallDirection;
        private bool m_IsMoveDisabled = false;

        private float m_JumpCharge = 0f;


        private void Awake()
        {
            // Setting up references.
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = true;
                m_Anim.SetBool("Ground", m_Grounded);
            }
            else if (collision.otherCollider == m_wallCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_WallDirection = collision.GetContact(0).point.x - transform.position.x;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = false;
            }
            else if (collision.otherCollider == m_wallCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_WallDirection = 0;
            }
        }

        private void FixedUpdate()
        {
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.linearVelocity.y);
            Move(m_MoveDirection);
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
            m_Anim.SetFloat("Speed", Mathf.Abs(direction));
            if (direction != 0 && (m_Grounded || m_AirControl))
            {

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
            else if (direction == 0 && !m_Grounded)
            {
                m_Rigidbody2D.linearVelocityX *= (float)Math.Exp(-m_AirDeceleration * Time.deltaTime);

                if (Mathf.Abs(m_Rigidbody2D.linearVelocityX) < 0.05f)
                {
                    m_Rigidbody2D.linearVelocityX = 0f;
                }
            }
            else if (direction == 0 && m_Grounded)
            {
                m_Rigidbody2D.linearVelocityX *= (float)Math.Exp(-m_Deceleration * Time.deltaTime);

                if (Mathf.Abs(m_Rigidbody2D.linearVelocityX) < 0.05f)
                {
                    m_Rigidbody2D.linearVelocityX = 0f;
                }
            }
        }

        public IEnumerator WallJumpTimer()
        {
            m_IsMoveDisabled = true;
            yield return new WaitForSeconds(m_WallJumpMoveDisableTime);
            m_IsMoveDisabled = false;
        }

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
        public void OnJump(InputAction.CallbackContext input)
        {
            if (input.performed)
            {
                if (m_Grounded)
                {
                    if (m_IsCrouching)
                    {
                        StartCoroutine(ChargeJump());
                    }
                    else
                    {
                        m_JumpCount = 0;
                        m_Grounded = false;
                        m_Anim.SetBool("Ground", false);
                        m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpForce);
                    }
                }
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
                else if (m_MaxJumpCount > m_JumpCount) // Double-Jump
                {
                    m_JumpCount++;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpForce);
                }
            }
            if (input.canceled)
            {
                if (m_JumpCharge != 0)
                {
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocityX, m_JumpCharge);
                }
            }
        }

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

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
