using UnityEngine;
using System.Collections;
using MouseController;
using GunController;
using SoundController;

namespace EquipedItemHandler
{
	public class EquipedItemHandler : MonoBehaviour 
	{

		public enum State
		{
			NOTHING,
			GUN,
			HAND
		};
		public State StartingState;
		private State _currentState = State.NOTHING;

		private Mouse_Controller _mouseController;
		private SpiritGun _gunController;
		private SoundManager _soundManager;

		public AudioSource flashlightSound;

		public Light _flashlight;
		public Light _GrabbablesLight;

		public bool FlashlightOn;
		public GameObject FlashlightLight;

		// Use this for initialization
		void Start () 
		{
			_mouseController = GameObject.FindGameObjectWithTag ("UiReticle").GetComponent<Mouse_Controller> ();
			_gunController = gameObject.GetComponentInChildren<SpiritGun> ();
			_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();

			flashlightSound.clip = _soundManager.GetSFXSound(2);
			
			
			if (StartingState == State.GUN && _currentState != State.GUN) 
			{
				_currentState = State.GUN;
				_mouseController.setEquipped(false);
				_gunController.setEquipped(true);
			}
			else if (StartingState == State.HAND && _currentState != State.HAND) 
			{
				_currentState = State.HAND;
				_mouseController.setEquipped(true);
				_gunController.setEquipped(false);
			}

			//Get flashlight
			//_flashlight = GetComponentInChildren<Light>();

			if (FlashlightOn) 
			{
				_flashlight.enabled = true;
				_GrabbablesLight.enabled = true;
			} 
			else 
			{
				_flashlight.enabled = false;
				_GrabbablesLight.enabled = false;
			}
		}
		
		// Update is called once per frame
		void Update () 
		{


			if (Input.GetKeyDown (KeyCode.Alpha1) && _currentState != State.GUN) 
			{
				_currentState = State.GUN;
				_mouseController.setEquipped(false);
				_gunController.setEquipped(true);
			}

			if (Input.GetKeyDown (KeyCode.Alpha2) && _currentState != State.HAND) 
			{
				_currentState = State.HAND;
				_mouseController.setEquipped(true);
				_gunController.setEquipped(false);
								
			}
			if (_currentState == State.GUN) UpdateGun();
			if (_currentState == State.HAND) UpdateHand();

			if (Input.GetButtonDown ("Flashlight")) {
				flashlightSound.Play();
				
				if (FlashlightOn) 
				{
					_flashlight.enabled = false;
					_GrabbablesLight.enabled = false;
					FlashlightOn = false;
					FlashlightLight.SetActive(false);
				} 
				else 
				{
					_flashlight.enabled = true;
					_GrabbablesLight.enabled = true;
					FlashlightOn = true;		
					FlashlightLight.SetActive(true);
				}
			}
		
		}


		void UpdateGun()
		{

		}

		void UpdateHand()
		{

		}

	}
}

