using System;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MoveForce = 50f;                   // Amount of force added to move the player left and right.
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_WallSpeed = -1f;                   // The fastest the player can travel in the y axis on a wall.
        [SerializeField] private float m_JumpForce = 800f;                  // Amount of force added when the player jumps.
        [SerializeField] private float m_MaxJumpForce = 1600f;              // Max amount of force added when the player jumps.
        [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;   // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsWall;                    // A mask determining what is wall to the character
        [SerializeField] private int m_MaxJumps = 1;                        // The maximum number of jumps the player can do in the air
        [SerializeField] private int m_TMaxSaut = 2;                        // The time in seconds from which the maximum amount of Jump Force is reached

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Transform m_WallCheck;      // A position marking where to check for walls.
        const float k_WalledRadius = .5f;   // Radius of the overlap circle to determine if the player is against a wall
        private bool m_Walled;              // Whether or not the player is against a wall.
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private int m_JumpsLeft = 0;        // The number of jumps the player has left

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_WallCheck = transform.Find("WallCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            m_Walled = false;
            Collider2D[] colliders2 = Physics2D.OverlapCircleAll(m_WallCheck.position, k_WalledRadius, m_WhatIsWall);
            for (int i = 0; i < colliders2.Length; i++)
            {
                if (colliders2[i].gameObject != gameObject)
                    m_Walled = true;
            }
        }


        public void Move2(float move2, bool move, float duration)
        {
            if (m_Grounded || m_AirControl)
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move2));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move2 * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move2 > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move2 < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            // Duration: en secondes, si on depasse m_TMaxSaut en s, on reste au maximum de saut,
            duration = Mathf.Min(duration, m_TMaxSaut);
            float durationRatio = duration / m_TMaxSaut;
            float jumpForce = Mathf.Max(durationRatio * m_MaxJumpForce, m_JumpForce);
            if (m_Grounded)
            {
                int test = move ? 1 : 0;
                m_Rigidbody2D.AddForce(new Vector2(0f, test * jumpForce));
            }
        }


        public void Move(float move, bool crouch, bool jump)
        {

            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move * m_CrouchSpeed : move);

                // Move the character by pushing a force into the rigidbody2D.
                m_Rigidbody2D.AddForce(m_MoveForce * new Vector2(move, 0));

                // Limit the speed of the character
                float x_vel = Mathf.Clamp(m_Rigidbody2D.velocity.x, -m_MaxSpeed, m_MaxSpeed);
                float y_vel = (m_Walled && move != 0) ? Mathf.Max(m_WallSpeed, m_Rigidbody2D.velocity.y) : m_Rigidbody2D.velocity.y;
                m_Rigidbody2D.velocity = new Vector2(x_vel, y_vel);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(x_vel));

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            // If the player should jump...
            if (jump)
            {
                // Log m_Walled and m_Grounded
                Debug.Log("m_Walled: " + m_Walled + " m_Grounded: " + m_Grounded);

                // And if the player is grounded, reset the number of jumps left
                if (m_Grounded && m_Anim.GetBool("Ground"))
                {
                    m_Anim.SetBool("Ground", false);
                    m_JumpsLeft = m_MaxJumps;
                }

                if (m_Walled && !m_Grounded)
                {
                    Debug.Log("Wall jump");
                    m_Rigidbody2D.AddForce(m_JumpForce * new Vector2(m_FacingRight ? -1 : 1, 1));
                    Flip();
                    m_JumpsLeft = m_MaxJumps;
                }
                else if (m_JumpsLeft > 0)
                {
                    Debug.Log("Classic jump");
                    // Reset y velocity
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                    // Add a vertical force to the player.
                    m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce);
                    // Decrement the number of jumps left
                    m_JumpsLeft--;
                    Debug.Log("Jumps left: " + m_JumpsLeft);
                }
                else
                {
                    Debug.Log("No jump");
                }
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
