using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using GunController;

namespace SimplePlayerCamera
{
	public class SimpleCamera : MonoBehaviour 
	{	
		
		public float XSensitivity = 2f;
		public float YSensitivity = 2f;
		public bool clampVerticalRotation = true;
		public float MinimumX = -90F;
		public float MaximumX = 90F;
		public bool smooth;
		public float smoothTime = 5f;
		
		
		private Quaternion m_CharacterTargetRot;
		private Quaternion m_CameraTargetRot;
		
		public GameObject Character;
		public GameObject MainCamera;

		private float xRot;
		private float yRot;


		void Start()
		{
	
		}
	
		//All of the camera updates are called from the mouse_controller
		
		
		Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;
			
			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
			
			angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
			
			q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
			
			return q;
		}
		
		
		//When we're not holding something we treat the mouse input like this.
		public void UpdateFreeCamera()
		{
			m_CharacterTargetRot = Character.transform.localRotation;
			m_CameraTargetRot = MainCamera.transform.localRotation;
			
			float yRot = Input.GetAxis("Mouse X") * XSensitivity;
			float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
			
			if(clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
			
			if(smooth)
			{
				Character.transform.localRotation = Quaternion.Slerp (Character.transform.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
				MainCamera.transform.localRotation = Quaternion.Slerp (MainCamera.transform.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
			}
			else
			{
				Character.transform.localRotation = m_CharacterTargetRot;
				MainCamera.transform.localRotation = m_CameraTargetRot;
			}
		}
		
		//How we treat input when we have grabbed something and need to look left or right.
		public void UpdateHorizontalCamera()
		{
			m_CharacterTargetRot = Character.transform.localRotation;
			m_CameraTargetRot = MainCamera.transform.localRotation;
			
			yRot = 0f;
			xRot = Input.GetAxis("Mouse Y") * YSensitivity;
			
			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
			
			if(clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
			
			if(smooth)
			{
				Character.transform.localRotation = Quaternion.Slerp (Character.transform.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
				MainCamera.transform.localRotation = Quaternion.Slerp (MainCamera.transform.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
			}
			else
			{
				Character.transform.localRotation = m_CharacterTargetRot;
				MainCamera.transform.localRotation = m_CameraTargetRot;
			}

		}
		
		//How we treat input when we have grabbed something and have to look up and down
		public void UpdateVerticalCamera()
		{
			m_CharacterTargetRot = Character.transform.localRotation;
			m_CameraTargetRot = MainCamera.transform.localRotation;
			
			yRot = Input.GetAxis("Mouse X") * XSensitivity;
			xRot = 0f;
			
			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
			
			if(clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
			
			if(smooth)
			{
				Character.transform.localRotation = Quaternion.Slerp (Character.transform.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
				MainCamera.transform.localRotation = Quaternion.Slerp (MainCamera.transform.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
			}
			else
			{
				Character.transform.localRotation = m_CharacterTargetRot;
				MainCamera.transform.localRotation = m_CameraTargetRot;
			}
			
		}

		public void ChangeX(){
			float value_ = GameObject.Find ("X_Sens").GetComponent <Slider> ().value;
			XSensitivity=value_;
				
		}
		
		public void ChangeY(){
			float value_ = GameObject.Find ("Y_Sens").GetComponent <Slider> ().value;
			YSensitivity=value_;
			
		}
		

	}
}
