using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace FirstPersonMobileTools.DynamicFirstPerson
{

	public class CameraLook : MonoBehaviour {	

		public enum TouchDetectMode {
			FirstTouch,
			LastTouch,
			All
		}

		[HideInInspector] public float Sensitivity_X { private get { return m_Sensitivity.x; } set { m_Sensitivity.x = value; }}
		[HideInInspector] public float Sensitivity_Y { private get { return m_Sensitivity.y; } set { m_Sensitivity.y = value; }}

		[SerializeField] private float m_BottomClamp = 90f;
		[SerializeField] private float m_TopClamp = 90f;	
		[SerializeField] private bool m_InvertX = false;
		[SerializeField] private bool m_InvertY = false;
		[SerializeField] private int m_TouchLimit = 10;
		[SerializeField] private Vector2 m_Sensitivity = Vector2.one;	
		public TouchDetectMode m_TouchDetectMode;
		
		private int m_TouchesDetectModeIndex;
		private float invertX, invertY;
		private float m_HorizontalRot;
		private float m_VerticalRot;
		private float m_ScreenWidth = Screen.width;
		private Func<Touch, bool> m_IsTouchAvailable;						// Delegate takes parameter touch and return true if touch is the available touch for camera rotation
		private List<string> m_AvailableTouchesId = new List<string>();		// Get all the touches that began without colliding with any UI Image/Button
		private EventSystem m_EventStytem;			
		private Transform m_CameraTransform;

		[HideInInspector] public Vector2 delta = Vector2.zero;

		private void Start() 
		{
			
			if (Camera.main != null)
				m_CameraTransform = Camera.main.transform;
			else Debug.LogError($"Can't find any main camera in scene!\n(Set your camera tag as MainCamera)", this);
			
			if (EventSystem.current != null)
				m_EventStytem = EventSystem.current;
			else Debug.LogError($"Scene has no Event System!");

			OnChangeSettings();
			
		} 

		private void Update() 
		{

			if (Input.touchCount == 0) return;
			foreach (var touch in Input.touches)
			{
				if ((touch.phase == TouchPhase.Began && m_EventStytem != null) &&
					!m_EventStytem.IsPointerOverGameObject(touch.fingerId) &&
					m_AvailableTouchesId.Count <= m_TouchLimit)
						m_AvailableTouchesId.Add(touch.fingerId.ToString());
				
				if (m_AvailableTouchesId.Count == 0) continue;	
				
				if (m_IsTouchAvailable(touch))
				{
					delta += new Vector2(touch.deltaPosition.x, touch.deltaPosition.y);
					if (touch.phase == TouchPhase.Ended) m_AvailableTouchesId.RemoveAt(0);
				} 
				else if (touch.phase == TouchPhase.Ended) m_AvailableTouchesId.Remove(touch.fingerId.ToString());
			}
			
		}

		private void LateUpdate()
		{
			
			m_HorizontalRot = delta.x * m_Sensitivity.x * Time.deltaTime * invertX;
			m_VerticalRot += delta.y * m_Sensitivity.y * Time.deltaTime * invertY;
			m_VerticalRot = Mathf.Clamp(m_VerticalRot, -m_BottomClamp, m_TopClamp);
			
			if (m_CameraTransform != null) m_CameraTransform.localRotation = Quaternion.Euler(m_VerticalRot, 0.0f, 0.0f);
			transform.Rotate(Vector3.up * m_HorizontalRot);

			delta = Vector2.zero;

		}

		public void OnChangeSettings()
		{
			
			invertX = m_InvertX? -1 : 1;
			invertY = m_InvertY? -1 : 1;
			
			switch (m_TouchDetectMode)
			{
				case TouchDetectMode.FirstTouch: 
					m_IsTouchAvailable = (Touch touch) => { return touch.fingerId.ToString() == m_AvailableTouchesId[0]; }; 
					break;
				case TouchDetectMode.LastTouch:
					m_IsTouchAvailable = (Touch touch) => { return touch.fingerId.ToString() == m_AvailableTouchesId[m_AvailableTouchesId.Count - 1]; };
					break;
				case TouchDetectMode.All:
					m_IsTouchAvailable = (Touch touch) => { return m_AvailableTouchesId.Contains(touch.fingerId.ToString()); };
					break;
			}

		}

		public void SetMode(int value)
		{
			switch (value)
			{
				case 0: 
					m_TouchDetectMode = TouchDetectMode.All;
					break;
				case 1: 
					m_TouchDetectMode = TouchDetectMode.LastTouch;
					break;
				case 2: 
					m_TouchDetectMode = TouchDetectMode.FirstTouch;
					break;
			}
		}

	}

}