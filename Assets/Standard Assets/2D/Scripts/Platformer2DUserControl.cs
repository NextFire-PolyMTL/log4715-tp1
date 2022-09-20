using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        float time_init = 0;
        private bool jump1;
        private bool crouch1;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            
            jump1 = CrossPlatformInputManager.GetButton("Jump");
            crouch1 = Input.GetKey(KeyCode.LeftControl);
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            if (!crouch && m_Jump){
                m_Character.Move(h, crouch, m_Jump);
                m_Jump = false;
            }
            else if (crouch && m_Jump){
                m_Character.Move(h, crouch, false);
                m_Jump = false;
            }
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
            bool saut_chargé = false;
            if (jump1 && crouch1)
            { 
                m_Character.Move(h, true, false);
                saut_chargé = false;
                time_init += Time.fixedDeltaTime;

            }
            else
            {
                if (time_init > 0)
                {
                    Debug.Log(time_init);
                    saut_chargé = true;
                    m_Character.Move2(h, saut_chargé, time_init);
                    Debug.Log("ca a marché");
                    time_init = 0;
                    saut_chargé = false;
                }
            }


        }
    }
}
