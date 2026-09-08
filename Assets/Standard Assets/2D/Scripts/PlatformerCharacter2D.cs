using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SpeedTree.Importer;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.PlayerLoop;
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
        [SerializeField] public Collider2D m_rightCollider;
        [SerializeField] public Collider2D m_leftCollider;

        //private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        //const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        //private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        //const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private int m_JumpCount = 0;
        private bool m_OnLeftWall;
        private bool m_OnRightWall;

        private bool m_IsCrouching = false;

        private float m_MoveDirection = 0f;


        private void Awake()
        {
            // Setting up references.
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    if (collision.gameObject.CompareTag("Wall"))
        //    {

        //    }
        //}

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = true;
                m_JumpCount = 0;
                m_Anim.SetBool("Ground", m_Grounded);
            }
            else if (collision.otherCollider == m_leftCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_OnLeftWall = true;
                m_JumpCount = 0;
            }
            else if (collision.otherCollider == m_rightCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_OnRightWall = true;
                m_JumpCount = 0;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.otherCollider == m_feetCollider && collision.gameObject.CompareTag("Ground"))
            {
                m_Grounded = false;
            }
            else if (collision.otherCollider == m_leftCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_OnLeftWall = false;
            }
            else if (collision.otherCollider == m_rightCollider && collision.gameObject.CompareTag("Wall"))
            {
                m_OnRightWall = false;
            }
        }

        private void FixedUpdate()
        {

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.linearVelocity.y);
            Move(m_MoveDirection);
        }




    


        public void OnMove(InputAction.CallbackContext input)
        {
            m_MoveDirection = input.ReadValue<Vector2>().x;
        }
        public void Move(float direction)
        {
            m_Anim.SetFloat("Speed", Mathf.Abs(direction));
            if (direction != 0 && (m_Grounded || m_AirControl))
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                direction = (m_IsCrouching ? direction * m_CrouchSpeed : direction);

                // The Speed animator parameter is set to the absolute value of the horizontal input.

                // Move the character
                m_Rigidbody2D.linearVelocity = new Vector2(direction * m_MaxSpeed, m_Rigidbody2D.linearVelocity.y);

                // If the input is moving the player right and the player is facing left...
                if (direction > 0 && !m_FacingRight)
                {
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (direction < 0 && m_FacingRight)
                {
                    Flip();
                }
            }
            else if (direction == 0 && (m_AirControl || m_Grounded))
            {
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
            }
        }

       public void OnCrouch(InputAction.CallbackContext input)
        {
            if (m_Grounded)
            {
                m_IsCrouching = input.ReadValue<bool>();
                m_Anim.SetBool("Crouch", input.ReadValue<bool>());
            }
        }
        public void OnJump(InputAction.CallbackContext input)
        {
            // If the player should jump...
            if (m_OnLeftWall)
            {
                // Add a vertical force to the player.
                Flip();
                m_OnLeftWall = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.linearVelocity = new Vector2(m_WallJumpHorizontalForce, m_JumpForce);
            }
            else if (m_OnRightWall)
            {
                // Add a vertical force to the player.
                Flip();
                m_OnRightWall = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.linearVelocity = new Vector2(-m_WallJumpHorizontalForce, m_JumpForce);
            }
            else if (m_Grounded)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_JumpForce);
                
            } else if (m_MaxJumpCount > m_JumpCount) // Double-Jump
            {
                m_JumpCount++;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_JumpForce);
            }
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
