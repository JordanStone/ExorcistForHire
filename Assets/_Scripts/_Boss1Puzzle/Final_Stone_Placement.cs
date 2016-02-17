using UnityEngine;
using System.Collections;
using DG.Tweening;
using NashTools;
using FinalStone;
using SoundController;
	
	/* This script works in junction with MagicStone.cs
 * 
 * The Purpose of this script is to allow the player to put 
 * magic stones on the final pedestals to beat the final boss.
 * 
 * Both scripts use a stone type enum to check to see if the right
 * stone is placed on the right pedestal.
 * 
*/
namespace FinalStone
{

	//These are all the ItemTypes the Stones/Pedestals Can have
	//They're outside of a class so we can use them in the MagicStone class as well.
	public enum ItemType
	{
		NONE,
		Earth,
		Wind,
		Fire,
		Water,
		Spirit
	}

	public class Final_Stone_Placement : MonoBehaviour 
	{

		public ItemType Type = ItemType.NONE;
		private float thrust = 2f;
	
		//The magic stone object that's going to be on the pedestal.
		// I know its not a "stone" per se just trust me on this.
		private GameObject Stone;

		//The time it takes for the object to go from your hand to the placement location when it enters the trigger.
		public float PlacementTime;

		//Make sure it doesn't get placed twice. It shouldn't but this is to be careful.
		private bool _placed = false;

		public AudioSource fireSound;
		private SoundManager _soundManager;

		// Use this for initialization
		void Start () 
		{
			_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
			fireSound.clip = _soundManager.GetSFXSound(3);

			EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);	
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		
		void OnReset(Component poster, object checkpointData)
		{
			//We aren't placed.
			_placed = false;

			//If we have a stone it's not kinematic anymore.
			if (Stone != null) 
			{
				Stone.GetComponent<Rigidbody>().isKinematic = false;
				Stone = null;
			}
			
		}

		//If an object enters the trigger.
		void OnTriggerEnter(Collider other) 
		{
			//If the object is a FinalObject
			if (other.gameObject.tag == "FinalObject" && !_placed) 
			{
				Stone= other.gameObject;
				Rigidbody StoneBody = Stone.GetComponent<Rigidbody>();
				//Get the magic stone's type, compare it to our type.
				if(Stone.GetComponent<MagicStone>().GetItemType()  == (Type))
				{
					//If they're the same we need to drop it
					StoneBody.isKinematic = true;

					//And move it to the transform of this gameobject, which is where the object belongs.
					StoneBody.DOMove(transform.position, PlacementTime);
					//Rotate it as well! You may need to rotate the actual gameobject this
					//script is attatched to to make this look proper.
					StoneBody.DORotate(transform.rotation.eulerAngles, PlacementTime);

					// Enable shrine point lights
					transform.GetChild(0).gameObject.SetActive(true);
					fireSound.Play();

					//Post a notification saying that somethings been placed.
					EventManager.PostNotification((int) GameManagerScript.GameEvents.StonePlaced, this, 1f);

					_placed = true;
				}
				else
				{
					Stone = null;
				}
			}
		}

		void OnTriggerStay(Collider other)
		{
			float upReduction = 2f;
			float forwardReduction = 2f;

			Rigidbody StoneBody = other.gameObject.GetComponent<Rigidbody>();

			if (other.gameObject.tag != "FinalObject" || !StoneBody.isKinematic ) // This isn't our object, get rid of it
			{
				if (StoneBody != null)
				{
					Vector3 rejectionDirection = (-transform.forward / forwardReduction) + (Vector3.up / upReduction);
					StoneBody.AddForce(rejectionDirection * thrust, ForceMode.Impulse);					
				}

			}
		}
		
	}
}
