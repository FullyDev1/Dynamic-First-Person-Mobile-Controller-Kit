using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FirstPersonMobileTools
{
        
    [RequireComponent(typeof(Image))]
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

        public enum JoystickMode {
            Fixed,
            FixedFloating,
            Float,
            Dynamic
        }
        
        public JoystickMode m_JoystickMode;
        [SerializeField] private RectTransform m_HandleRectTransform;
        [SerializeField] private RectTransform m_HandleBaseRectTransform;
        [SerializeField] private RectTransform m_JoystickTouchArea;
        public RectTransform TestRect;

        private delegate void JoystickInputDelegate(out Vector2 Position, PointerEventData EventData);
        private JoystickInputDelegate m_JoystickInputDelegate;
        private Image m_JoystickTouchAreaImage;
        private Vector2 m_JoystickTouchAreaMinPosition;
        private Vector2 m_JoystickTouchAreaMaxPosition;
        private Vector2 m_OriginalPosition;
        private Vector2 m_FixedFloatPivot;
        private float m_ScreenHeight = Screen.height;
        private bool m_PointerIsInJoystickArea;

        Canvas m_Canvas;

        [HideInInspector] public float Horizontal { get; private set; } 
        [HideInInspector] public float Vertical { get; private set; }
        [HideInInspector] public Vector2 InputLocalPosition;

        private void Start()
        {

            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null)
                Debug.LogError($"Joystick is not in the canvas!", this); 

            m_JoystickTouchAreaImage = m_JoystickTouchArea.GetComponent<Image>();
            m_OriginalPosition = m_HandleBaseRectTransform.anchoredPosition;

            Vector2 AnchorCenter = Vector2.one / 2;
            m_HandleRectTransform.pivot = AnchorCenter;
            m_HandleRectTransform.anchorMax = AnchorCenter;
            m_HandleRectTransform.anchorMin = AnchorCenter;
            m_HandleRectTransform.anchoredPosition = Vector2.zero;

            m_OriginalPosition = m_HandleBaseRectTransform.anchoredPosition;

            m_JoystickTouchAreaMinPosition = 
                new Vector2(m_JoystickTouchArea.anchoredPosition.x, m_JoystickTouchArea.anchoredPosition.y) * m_Canvas.scaleFactor;
            m_JoystickTouchAreaMaxPosition = 
                new Vector2(m_JoystickTouchArea.anchoredPosition.x + m_JoystickTouchArea.rect.size.x,
                            m_JoystickTouchArea.anchoredPosition.y + m_JoystickTouchArea.rect.size.y) * m_Canvas.scaleFactor;;

            OnPointerUp(null);

            OnChangeSettings();
        }

        public void OnPointerDown(PointerEventData EventData) 
        {

            m_PointerIsInJoystickArea = true;
            if (m_JoystickMode != JoystickMode.Fixed)
            {  
                if (m_JoystickMode == JoystickMode.FixedFloating)
                {
                    m_FixedFloatPivot = EventData.position;
                }
                else 
                {
                    m_HandleBaseRectTransform.gameObject.SetActive(true);
                    m_HandleBaseRectTransform.anchoredPosition = EventData.position / m_Canvas.scaleFactor;
                }
            }
            
            OnDrag(EventData);

        }

        public void OnPointerUp(PointerEventData EventData)
        {

            if (m_JoystickMode != JoystickMode.Fixed && m_JoystickMode != JoystickMode.FixedFloating) 
            {
                m_HandleBaseRectTransform.gameObject.SetActive(false);
                m_HandleBaseRectTransform.anchoredPosition = m_OriginalPosition;
            }
            m_HandleRectTransform.anchoredPosition = Vector2.zero;

            Horizontal = 0f;
            Vertical = 0f;

        }

        public void OnDrag(PointerEventData EventData)
        {
            if (m_JoystickMode != JoystickMode.Fixed && m_PointerIsInJoystickArea)
            {
                m_PointerIsInJoystickArea = EventData.position.x >= m_JoystickTouchAreaMinPosition.x && 
                                            EventData.position.x <= m_JoystickTouchAreaMaxPosition.x &&
                                            m_ScreenHeight - EventData.position.y >= m_JoystickTouchAreaMinPosition.y &&
                                            m_ScreenHeight - EventData.position.y <= m_JoystickTouchAreaMaxPosition.y;
            }

            if (!m_PointerIsInJoystickArea)
            {
                OnPointerUp(EventData);
                return;
            }

            m_JoystickInputDelegate(out InputLocalPosition, EventData);

            m_HandleRectTransform.anchoredPosition = InputLocalPosition;
            
            Horizontal = (float)Math.Round(InputLocalPosition.x / (m_HandleBaseRectTransform.sizeDelta.x / 2f), 3);
            Vertical = (float)Math.Round(InputLocalPosition.y / (m_HandleBaseRectTransform.sizeDelta.y / 2f), 3);

        }

        public void OnChangeSettings()
        {        
            
            switch (m_JoystickMode)
            {
                case JoystickMode.Fixed:
                    JoystickFixedMode();
                    break;

                case JoystickMode.FixedFloating:
                    JoystickFixedFloatingMode();
                    break;

                case JoystickMode.Float:
                    JoystickFloatMode();
                    break;

                case JoystickMode.Dynamic:
                    JoystickDynamicMode();
                    break;
            }

        }    

        private void JoystickFixedMode()
        {
            m_HandleBaseRectTransform.gameObject.SetActive(true);
            if (m_OriginalPosition != Vector2.zero) m_HandleBaseRectTransform.anchoredPosition = m_OriginalPosition;
            if (m_JoystickTouchAreaImage != null) m_JoystickTouchAreaImage.enabled = false;

            m_JoystickInputDelegate = (out Vector2 InputLocalPosition, PointerEventData EventData) => {
                Vector2 HandleBasePositionInScreen = m_HandleBaseRectTransform.anchoredPosition * m_Canvas.scaleFactor; 
                InputLocalPosition = (EventData.position - HandleBasePositionInScreen) / m_Canvas.scaleFactor; 
                
                if (InputLocalPosition.magnitude > m_HandleBaseRectTransform.sizeDelta.x / 2f)
                    InputLocalPosition = InputLocalPosition.normalized * (m_HandleBaseRectTransform.sizeDelta / 2f);
            };
        }

        private void JoystickFixedFloatingMode()
        {
            m_HandleBaseRectTransform.gameObject.SetActive(true);
            if (m_OriginalPosition != Vector2.zero) m_HandleBaseRectTransform.anchoredPosition = m_OriginalPosition;
            if (m_JoystickTouchAreaImage != null) m_JoystickTouchAreaImage.enabled = true;

            m_JoystickInputDelegate = (out Vector2 InputLocalPosition, PointerEventData EventData) => {
                Vector2 HandleBasePositionInScreen = m_HandleBaseRectTransform.anchoredPosition * m_Canvas.scaleFactor; 
                InputLocalPosition = (EventData.position - m_FixedFloatPivot) / m_Canvas.scaleFactor;
                
                if (InputLocalPosition.magnitude > m_HandleBaseRectTransform.sizeDelta.x / 2f)
                    InputLocalPosition = InputLocalPosition.normalized * (m_HandleBaseRectTransform.sizeDelta / 2f);
            };
        }

        private void JoystickFloatMode()
        {
            m_HandleBaseRectTransform.gameObject.SetActive(false);
            if (m_JoystickTouchAreaImage != null) m_JoystickTouchAreaImage.enabled = true;

            m_JoystickInputDelegate = (out Vector2 InputLocalPosition, PointerEventData EventData) => {
                Vector2 HandleBasePositionInScreen = m_HandleBaseRectTransform.anchoredPosition * m_Canvas.scaleFactor; 
                InputLocalPosition = (EventData.position - HandleBasePositionInScreen) / m_Canvas.scaleFactor;  
                
                if (InputLocalPosition.magnitude > m_HandleBaseRectTransform.sizeDelta.x / 2f)
                    InputLocalPosition = InputLocalPosition.normalized * (m_HandleBaseRectTransform.sizeDelta / 2f);
            };
        }

        private void JoystickDynamicMode()
        {
            m_HandleBaseRectTransform.gameObject.SetActive(false);
            if (m_JoystickTouchAreaImage != null) m_JoystickTouchAreaImage.enabled = true;

            m_JoystickInputDelegate = (out Vector2 InputLocalPosition, PointerEventData EventData) => {
                Vector2 HandleBasePositionInScreen = m_HandleBaseRectTransform.anchoredPosition * m_Canvas.scaleFactor; 
                InputLocalPosition = (EventData.position - HandleBasePositionInScreen) / m_Canvas.scaleFactor; 
                
                if (InputLocalPosition.magnitude > m_HandleBaseRectTransform.sizeDelta.x / 2f)
                {
                    m_HandleBaseRectTransform.anchoredPosition += 
                        InputLocalPosition - (InputLocalPosition.normalized * m_HandleBaseRectTransform.sizeDelta / 2);

                    InputLocalPosition = InputLocalPosition.normalized * (m_HandleBaseRectTransform.sizeDelta / 2f);
                }
            };
        }

        // Accessible function through setting to change joystick mode
        public void SetMode(int value)
        {
            switch (value)
            {
                case 0: 
                    m_JoystickMode = JoystickMode.Fixed;
                    break;
                case 1: 
                    m_JoystickMode = JoystickMode.FixedFloating;
                    break;
                case 2: 
                    m_JoystickMode = JoystickMode.Float;
                    break;
                case 3: 
                    m_JoystickMode = JoystickMode.Dynamic;
                    break;
            }
        }

    }

}