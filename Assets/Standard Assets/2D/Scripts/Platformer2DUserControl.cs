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
        private bool jump0;
        private bool jump1;
        private bool jump2;
        private bool crouch0;
        private bool crouch1;
        private bool crouch2;

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
            jump0 = CrossPlatformInputManager.GetButtonDown("Jump");
            jump1 = CrossPlatformInputManager.GetButton("Jump");
            jump2 = CrossPlatformInputManager.GetButtonUp("Jump");
            crouch0 = Input.GetKeyDown(KeyCode.LeftControl);
            crouch1 = Input.GetKey(KeyCode.LeftControl);
            crouch2 = Input.GetKeyUp(KeyCode.LeftControl);


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
            //if (jump1 && crouch0 ){
            //    float time_init=Time.time;
            //    saut_chargé=false;
            //    m_Character.Move(h, false, true);
            //    Debug.Log("ca a un peu marché");
            //}
            if (jump1 && crouch1)
            { //&&
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

//if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space)){
//                bool crouch2=Input.GetKeyDown(KeyCode.LeftControl);
//                bool jump=Input.GetKeyDown(KeyCode.Space);
//                float time_while_crouched_and_jumped=0;
//                float time_init=Time.time;
//                float move_p = CrossPlatformInputManager.GetAxis("Horizontal");
//                bool saut_chargé_actif=false;
//                if (crouch2 && jump && saut_chargé_actif==false){
//                    saut_chargé_actif=true;
//                }
//                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Space) && saut_chargé_actif==true){
//                    m_Character.Move(move_p,true,false);
//                }
//                if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.Space)) && saut_chargé_actif==true){
//                    time_while_crouched_and_jumped=Time.time-time_init;
//                    m_Character.Move2(move_p,time_while_crouched_and_jumped);
//                }
//            }
