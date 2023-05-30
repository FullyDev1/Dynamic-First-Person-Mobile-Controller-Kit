using UnityEngine;
using FirstPersonMobileTools.DynamicFirstPerson;

namespace FirstPersonMobileTools.Utility
{

    [RequireComponent(typeof(MovementController))]
    public class FovKick : MonoBehaviour {
            
        public float m_Ammount = 10.0f;
        public float m_Delay = 1.0f;

        [HideInInspector] public float m_OriginalFov = 0;

        private Camera m_Camera;
        private MovementController m_MovementController;
        private float m_CurrentFov;

        private bool m_Sprint
        {
            get { return m_MovementController.Input_Sprint; }
        }

        private bool m_Crouch
        {
            get { return m_MovementController.Input_Crouch; }
        }

        public void Start()
        {
            m_Camera = GetComponentInChildren<Camera>();
            m_MovementController = GetComponent<MovementController>();

            m_OriginalFov = m_Camera.fieldOfView;
            m_CurrentFov = m_OriginalFov;
        }

        private void FixedUpdate() 
        {

            if (m_Sprint && !m_Crouch && m_Camera.fieldOfView != m_OriginalFov + m_Ammount)
            {
                AdjustFov(Time.deltaTime);
            }
            
            if ((!m_Sprint || m_Crouch) && m_Camera.fieldOfView != m_OriginalFov)
            {
                AdjustFov(-Time.deltaTime);
            }

        }

        public void AdjustFov(float time)
        {   

            m_CurrentFov += (m_Ammount / m_Delay ) * time;
            m_CurrentFov = Mathf.Clamp(m_CurrentFov, m_OriginalFov, m_OriginalFov + m_Ammount);
            m_Camera.fieldOfView = m_CurrentFov;

        }

    }

}