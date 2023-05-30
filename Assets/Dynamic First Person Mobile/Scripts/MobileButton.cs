using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FirstPersonMobileTools
{

    [RequireComponent(typeof(Image))]
    public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        
        public enum ButtonMode {
            SingleTap,
            Toggle,
            PressAndRelease,
        }

        public Image m_Image;
        public Color m_OnPressedTransition = Color.white;
        public Color m_OnReleasedTransition = Color.white; 
        public float m_TransitionDuration = 0.1f;
        public ButtonMode m_ButtonMode;

        [SerializeField] private UnityEvent m_OnButtonPressed;
        [SerializeField] private UnityEvent m_OnButtonReleased;
        [SerializeField] private UnityEvent m_OnToggleOn;
        [SerializeField] private UnityEvent m_OnToggleOff;
        [SerializeField] private UnityEvent m_OnClicked;
        [SerializeField] private UnityEvent m_OnAfterClicked;

        private float m_TransitionTimeElapse;
        private bool m_isTogglePressed;
        private Action m_OnPointerDownAction;
        private Action m_OnPointerUpAction;
        private Coroutine TransitionCoroutine = null;

        private void Start() 
        {
            OnChangeSettings();
        }

        public void OnPointerDown(PointerEventData EventData)
        {
            m_OnPointerDownAction?.Invoke();
            if (TransitionCoroutine != null) StopCoroutine(TransitionCoroutine);
            TransitionCoroutine = StartCoroutine(Transition(m_OnPressedTransition));
        }

        public void OnPointerUp(PointerEventData EventData)
        {
            m_OnPointerUpAction?.Invoke();
            if (TransitionCoroutine != null) StopCoroutine(TransitionCoroutine);
            TransitionCoroutine = StartCoroutine(Transition(m_OnReleasedTransition));
        }

        private IEnumerator Transition(Color TransitionColor)
        {
            m_TransitionTimeElapse = m_TransitionDuration - m_TransitionTimeElapse;
            while (m_TransitionTimeElapse > 0)
            {
                m_Image.color = Color.Lerp(m_Image.color, TransitionColor, Time.deltaTime / m_TransitionTimeElapse);
            
                m_TransitionTimeElapse -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            m_Image.color = TransitionColor;
            m_TransitionTimeElapse = 0;

            yield return null;
        } 

        public void GetImage()
        {
            m_Image = GetComponent<Image>();
        }

        public void OnChangeSettings() 
        {
            
            switch (m_ButtonMode)
            {
                case ButtonMode.SingleTap:
                    m_OnPointerDownAction = () => { StartCoroutine(SingleTap()); };
                    m_OnPointerUpAction = null;
                    break;
                case ButtonMode.Toggle:
                    m_OnPointerDownAction = () => {
                        if (!m_isTogglePressed) { m_OnToggleOn?.Invoke(); m_isTogglePressed = true; }
                        else { m_OnToggleOff?.Invoke(); m_isTogglePressed = false; }
                    };
                    m_OnPointerUpAction = null;
                    break;

                case ButtonMode.PressAndRelease:
                    m_OnPointerUpAction = () => { m_OnButtonReleased?.Invoke(); };
                    m_OnPointerDownAction = () => { m_OnButtonPressed?.Invoke(); };
                    break;
            }

        }

        private IEnumerator SingleTap()
        {
            m_OnClicked?.Invoke();
            yield return new WaitForSeconds(Time.deltaTime);
            m_OnAfterClicked?.Invoke();
        }

    }

}
