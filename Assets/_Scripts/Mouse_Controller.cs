using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using SimplePlayerCamera;
using NashTools;

namespace MouseController
{
	public class Mouse_Controller : MonoBehaviour 
	{
		//Is the hand equipped?
		private bool _equiped = true;

		[Header("Flip Door Controls, (are doors opening backwards?)")]
		public bool Flip = false;

		[Header("Textures")]
		public Sprite OpenHand;
		public Sprite ClosedHand;
		public Sprite ReadyHand;
		public Sprite DoorHand;
		public Sprite KeyHand;
		public Sprite GunReticle;


		private Image _myImageScript;
		private Vector3 _startScale;
		[Header("For DoorHand how much should it scale up.")]
		public float SmallScaleIncrease = 2f;
		
		//The script that rotates the camera.
		private SimpleCamera _myPlayerCamera;

		//The player
		private GameObject _player;
		private Rigidbody _playerRigidbody;

		[Header("Hand Movement Sensitivity")]
		//Sensitivity for when we're grabbing something.
		public float HandXSensitivity;
		public float HandYSensitivity;
		
		//For setting the boundries the on screen hand can move.
		private float _screenLengthX;
		private float _screenLengthY;
		
		[Header("Hand boundries while grabbing.")]
		//The boundries in a percent(of screen space) of where the on screen hand cam move.
		public float OuterRectY;
		public float OuterRectX;
		
		//The bounds the on screen hand can move, calculated using the above two variables.w
		private float _handTopBounds;
		private float _handBottomBounds;
		private float _handLeftBounds;
		private float _handRightBounds;
		
		
		[Header("Grabbing Variables")]
		public static bool GrabbingDoor;
		//How far away things we can grab can be.
		public float MaxGrabLength;
		
		//If something we grab is closer than this, we set _grabbedDistance to this.
		public float MinGrabLength;
		public float GrabBreakForce;
		
		//A layer of things we can grab, will probably delete this later since it seems to be uneccesarry and will cause issues.
		public LayerMask Grabbables;
		//The object we spawn, this object is the "hand" that picks the object up.
		public GameObject InstantiatedHand;
		//This keeps track of the currently spawned "hand" We keep track of it so we can delete it when we let go of an object. 
		private GameObject _currentInstatiatedHand;
		private ConfigurableJoint _CurrentGrabbedObjectConfigurableJoint;
		
		//Time for the hand to recenter after grabbing.
		public float HandRecenterTime;
		
		public float SlerpSpring;
		public float SlerpDamp;
		
		//Grabbed Object velocity, we apply this when we let go.
		private Vector3 _grabbedVelocity;
		private Vector3 _grabbedObjectPositionLastFrame;
		
		
		//When we grab something, how far away it was.
		private float _grabbedDistance;
		
		//If we grab a door
		private HingeJoint _grabbedDoor;
		public float DoorForce;
		public float DoorTargetVelocity;

		//If we grab a pushable box
		private GameObject _grabbedBox;
		private SpringJoint[] _grabbedBoxSprings;
		
		[Header("Pushing Spring Variables")]
		public float PushingSpring = 2400f;
		public float PushingDamper = 800f;
		public float PushingBreakForce = 50f;
		public float PushingBreakDistance = 7f;
		public Vector3 PushingAnchor;

		[Header("Ladder Variables")]
		public float LadderSnapTime = .8f;
		public float RungLengthMeters = .4572f;
		public float LadderRungClimbTime = .5f;
		//How far forward to move at the top of the ladder.
		public float LadderTopForwardInMeters = 2f;
		public LayerMask LadderRaycasts;

		private GameObject _ladderHands;

		private Animator _ladderAnimations;

		//Inputs from the mouse.
		private float _xInput;
		private float _yInput;
		private bool _leftMouseDown;
		private bool _leftMouse;
		//For moving the mouse, the wanted transform of the hand.
		private Vector3 _wantedTransform;
		//Don't let the cam update twice a frame with the same inputs that'd be bad.
		private bool _camUpdatedThisFrame;
		
		
		//Rect Transform, where the hand is on the screen.
		private RectTransform _handTransform;
		//Where the hand starts on screen, keeping track of this helps us when we let go of an object, it helps us move the hand back to where it starts.
		private Vector3 _startingHandTransform;
		
		enum State
		{
			Free,
			GrabbingObject,
			GrabbingDoor,
			GrabbingPushable,
			Locked
		};
		private State CurrentState;
		
		
		// Use this for initialization
		void Start () 
		{
			//Set GrabbingDoor to false.
			GrabbingDoor = false;

			//Get the player
			_player = GameObject.Find("Player");
			//Get the Playercamera
			_myPlayerCamera = _player.GetComponent<SimpleCamera>();
			//Get the player's rigidbody
			_playerRigidbody = _player.GetComponent<Rigidbody>();

			//Free the cursor.
			CurrentState = State.Free;
			
			//Set _startScale
			_startScale = transform.localScale;
			
			//Set the hands position to go when Free
			_handTransform = gameObject.GetComponent<RectTransform>();
			_startingHandTransform = _handTransform.transform.position;
			
			//Set the bounds of where the hand can go on screen.
			setHandBounds ();
			
			//Get the image script we're using.
			_myImageScript = GetComponent<Image>();
			
			
			//Set the image to the gun reticle if we're not using the hand.
			if (_equiped == false) 
			{
				_myImageScript.sprite = GunReticle;
			}

			_ladderHands = GameObject.Find("LadderAnimations");
			_ladderAnimations = _ladderHands.GetComponent<Animator>();
			_ladderHands.SetActive(false);
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (!GameManagerScript.isPaused) 
			{
				GetInput ();
				if (CurrentState == State.Free)
					UpdateFree ();
				if (CurrentState == State.GrabbingPushable)
					UpdateGrabbingPushable();
				if (CurrentState == State.Locked)
					UpdateLocked();
			}
		}
	

		void GetInput()
		{
			_xInput = Input.GetAxis ("Mouse X");
			_yInput = Input.GetAxis ("Mouse Y");
			_leftMouseDown = Input.GetButtonDown ("Fire1");
			_leftMouse = Input.GetButton("Fire1");
			_leftMouse = Input.GetButton ("Fire1");
		}
		
		#region StateUpdates
		//Locked for cutscenes
		void UpdateLocked()
		{

		}


		//When we're not holding something we treat the mouse input like this.
		void UpdateFree()
		{
			if( _xInput != 0f ||  _yInput != 0f)
			{		
				_myPlayerCamera.UpdateFreeCamera();
			}
					
			//When I click, I'm trying to grab something. I can only do that if im equipped
			if(_equiped)
			{
				
				Ray GrabRay = Camera.main.ScreenPointToRay(_startingHandTransform);
				RaycastHit GrabInfo;
				//Raycast from hand center, if I hit something I can grab, grab it and switch to grabbing.
				if(Physics.Raycast(GrabRay, out GrabInfo, MaxGrabLength, Grabbables))
				{
					//Set the hand size to normal
					transform.localScale = _startScale;

					//If we can grab something switch to the grabbed hand.
					if(GrabInfo.transform.gameObject.tag == "Grabbable" ||GrabInfo.transform.gameObject.tag == "Key" || GrabInfo.transform.gameObject.tag == "DoorHandleRight"|| GrabInfo.transform.gameObject.tag == "DoorHandleLeft" || GrabInfo.transform.gameObject.tag == "Note" || GrabInfo.transform.gameObject.tag == "Pushable"|| GrabInfo.transform.gameObject.tag == "Shelf" || GrabInfo.transform.gameObject.tag == "FinalObject" || GrabInfo.transform.gameObject.tag == "Ladder" || GrabInfo.transform.gameObject.tag == "AmmoBox" )
					{
						_myImageScript.sprite = ReadyHand;
					}
					else if(GrabInfo.transform.gameObject.tag == "LockWithKeyInInv")
					{	
						_myImageScript.sprite = KeyHand;
						transform.localScale = _startScale * SmallScaleIncrease;
					}

					
					//If we click we grab it.
					if(_leftMouseDown)
					{
						//We can only grab things tagged with Grabbable!
						if(GrabInfo.transform.gameObject.tag == "Grabbable" || GrabInfo.transform.gameObject.tag == "FinalObject")
						{
							StartCoroutine(GrabGrabbable(GrabInfo));
						}
						//We can also grab keys, they go in our inventory though.
						if(GrabInfo.transform.gameObject.tag == "Key" || GrabInfo.transform.gameObject.tag == "Note" || GrabInfo.transform.gameObject.tag == "AmmoBox")
						{
							//We set it inactive.
							GrabInfo.transform.gameObject.SetActive(false);
						}
						
						//If we grabbed the right door handle.
						if(GrabInfo.transform.gameObject.tag == "DoorHandleRight")
						{
							_grabbedDoor = GrabInfo.transform.parent.gameObject.GetComponent<HingeJoint>();
							StartCoroutine(GrabDoor(GrabInfo, true));
						}
						//Left Door handle
						if(GrabInfo.transform.gameObject.tag == "DoorHandleLeft")
						{
							_grabbedDoor = GrabInfo.transform.parent.gameObject.GetComponent<HingeJoint>();
							StartCoroutine(GrabDoor(GrabInfo, false));
						}

						if(GrabInfo.transform.gameObject.tag == "Pushable")
						{
							StartCoroutine(GrabPushable(GrabInfo));
						}

						if(GrabInfo.transform.gameObject.tag == "Shelf")
						{
							GrabShelf(GrabInfo);
						}

						if(GrabInfo.transform.gameObject.tag == "LockWithKeyInInv")
						{
							EventManager.PostNotification((int) GameManagerScript.GameEvents.UnlockDoor, this, GrabInfo.transform.gameObject);
						}

						if(GrabInfo.transform.gameObject.tag == "NPC")
						{
							GrabInfo.transform.gameObject.GetComponent<Subtitles>().DisplayLine();
						}
						
						if(GrabInfo.transform.gameObject.tag == "Agent")
						{
							GrabInfo.transform.gameObject.GetComponent<Agent>().DisplayLine();
						}

						if(GrabInfo.transform.gameObject.tag == "Ladder")
						{
							StartCoroutine(GrabLadder(GrabInfo));
						}

					}
				}
				else
				{
					_myImageScript.sprite = OpenHand;
				}
				
			}
			
			
		}

		void UpdateGrabbingPushable()
		{
			/*
			if( _xInput != 0f ||  _yInput != 0f)
			{		
				_myPlayerCamera.UpdateFreeCamera();
			}
			*/
		}

		#endregion



		#region Coroutines
		IEnumerator GrabLadder(RaycastHit GrabInfo)
		{
			CurrentState =State.Locked;
			EventManager.PostNotification((int) GameManagerScript.GameEvents.Ladder, this, true);

			//Get the LadderBottom and LadderTop transforms to help
			Transform[] LadderChildren = GrabInfo.transform.parent.GetComponentsInChildren<Transform>();
			GameObject ladderBottom = null;
			GameObject ladderTop = null;

			//Raycast hieght for grounded, Character controller height, + Runglengthmeters. This is for going down.
			float rayDistance = _player.GetComponent<CharacterController>().height * .5f + RungLengthMeters;

			_ladderHands.SetActive(true);

			foreach (Transform Object in LadderChildren)
			{
				if(Object.name == "LadderBottom")
				{
					ladderBottom = Object.gameObject;
				}
				if(Object.name == "LadderTop")
				{
					ladderTop = Object.gameObject;
				}
			}

			//The time to move to the ladder, this should normalize it so we always move at the same speed.
			float _ladderSnapTime = (GrabInfo.distance / MaxGrabLength) * LadderSnapTime;

			//Bool to check which part of the ladder we're grabbing.
			bool GrabbingBottom = false;



			if(GrabInfo.transform.name == "LadderBottom")
			{
				GrabbingBottom = true;
				_player.transform.DOMove(ladderBottom.transform.position, _ladderSnapTime);
				_player.transform.DORotate(ladderBottom.transform.eulerAngles, _ladderSnapTime);
				Camera.main.transform.DORotate(new Vector3(0f,  ladderBottom.transform.eulerAngles.y, 0f), _ladderSnapTime);
				//set animator
				_ladderAnimations.SetTrigger("Enter Ladder");
				_ladderAnimations.speed = 1f;
			}
			else
			{
				_player.transform.DOMove(ladderTop.transform.position, _ladderSnapTime);
				_player.transform.DORotate(ladderTop.transform.eulerAngles, _ladderSnapTime);
				Camera.main.transform.DORotate(new Vector3(0f,  ladderTop.transform.eulerAngles.y, 0f), _ladderSnapTime);
				//set animator
				_ladderAnimations.SetTrigger("Enter Ladder");
				_ladderAnimations.speed = 1f;
			}



			/*
			 	public float LadderSnapTime = .8f;
				public float RungLengthMeters = 18f;
				public float LadderRungClimbTime = .5f;

			*/

			bool climbing = true;
			bool jumpQueud = false;
			bool climbQueud = false;

			float climbTimer = .41f;
			float vertAxis;

			while(climbing == true && jumpQueud == false)
			{

				vertAxis = Input.GetAxis("Vertical");

				if(vertAxis != 0f && climbTimer <= 0f)
				{
					//First check to see if we'd move too high or too low.
						//Too Low.
					if(Mathf.Sign(vertAxis) == -1)
					{
						//Raycast down.
						if(Physics.Raycast(_player.transform.position, Vector3.down, rayDistance))
						{
							climbing = false;
						}
					}
						//Too High
					if(Mathf.Sign(vertAxis) == 1)
					{

						//Raycast Forward, if we don't hit the ladder we're at the top.
						if(!Physics.Raycast(_player.transform.position + new Vector3(0f, rayDistance, 0f), _player.transform.forward, 3f,LadderRaycasts))
						{
							_ladderAnimations.SetTrigger("Top");
							//Moveforward off the ladder
							Tween MoveOffTween =_player.transform.DOMove(_player.transform.position + _player.transform.forward * LadderTopForwardInMeters + new Vector3(0f, rayDistance, 0f), 1f);
							yield return MoveOffTween.WaitForCompletion();

							climbing = false;
						}
					}

					if(climbing == true)
					{
						//Move Up or Down based on Axis.
						_player.transform.DOLocalMoveY( _player.transform.position.y + RungLengthMeters * Mathf.Sign(vertAxis), LadderRungClimbTime);
						//Since We moved set climbTimer
						climbTimer = LadderRungClimbTime;
					}
				}

				
				if(climbTimer >= 0f)
				{
					_ladderAnimations.speed = 1f;
				}
				else
				{
					_ladderAnimations.speed = 0f;
				}

				if(Input.GetButtonDown("Jump"))
				{
					jumpQueud = true;
				}

				climbTimer -= Time.deltaTime;

				yield return null;
			}

			CurrentState =State.Free;
			EventManager.PostNotification((int) GameManagerScript.GameEvents.Ladder, this, false);
			_ladderHands.SetActive(false);
		}			

		IEnumerator GrabGrabbable(RaycastHit GrabInfo)
		{

			//Wait for fixedupate to update this co-routine. This keeps the physics smooth.
			yield return new WaitForFixedUpdate();
			//I use a configurable joint to grab things so I need to put one on the thing I grabbed.
			GameObject GrabbedObject = GrabInfo.transform.gameObject;
			GrabbedObject.AddComponent<ConfigurableJoint>();
			_CurrentGrabbedObjectConfigurableJoint = GrabbedObject.GetComponent<ConfigurableJoint>();
			
			//Set grab distance.
			_grabbedDistance = GrabInfo.distance;
			
			if(_grabbedDistance < MinGrabLength)
			{
				_grabbedDistance = MinGrabLength;
			}
			
			//instantiate an object at the colision point, this is the connected body.
			_currentInstatiatedHand = Instantiate(InstantiatedHand, GrabInfo.point, Quaternion.LookRotation(GrabInfo.normal, Vector3.up)) as GameObject;
			
			
			//We need to lock X Y and Z motion on the configurable joint.
			_CurrentGrabbedObjectConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
			_CurrentGrabbedObjectConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
			_CurrentGrabbedObjectConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;
			_CurrentGrabbedObjectConfigurableJoint.breakForce = GrabBreakForce;
			//Set the connected rigidbody to the spawned hand's rigidbody.
			_CurrentGrabbedObjectConfigurableJoint.connectedBody = _currentInstatiatedHand.GetComponent<Rigidbody>();
			
			//Set the image to grabbing.
			_myImageScript.sprite = ClosedHand;
			
			//Set the state to grabbing.
			CurrentState = State.GrabbingObject;

			//Start the loop
			while (_leftMouse && _equiped && _CurrentGrabbedObjectConfigurableJoint)
			{
				_camUpdatedThisFrame = false;
				
				//If we're trying to move our hand left and we're NOT already at the left bounds we can move.
				if (_xInput > 0f && !(_handTransform.transform.position.x > _handRightBounds)) 
				{
					_wantedTransform.x = _xInput * HandXSensitivity * Time.deltaTime;
				}
				//If we're trying to move our hand right and we're NOT already at the right bounds we can move.
				else if (_xInput < 0f && !(_handTransform.transform.position.x < _handLeftBounds)) 
				{
					_wantedTransform.x = _xInput * HandXSensitivity * Time.deltaTime;
				} 
				//If we're at the left or right bounds we rotate the camera instead of moving the hand.
				else if (_xInput != 0f)
				{
					_camUpdatedThisFrame = true;
					_myPlayerCamera.UpdateFreeCamera();
				}
				//If we're trying to move our hand up and we're NOT already at the top bounds we can move.
				if (_yInput > 0f && !(_handTransform.transform.position.y > _handTopBounds)) 
				{
					_wantedTransform.y = _yInput * HandYSensitivity * Time.deltaTime;
				}
				//If we're trying to move our hand down and we're NOT already at the bottom bounds we can move.
				else if (_yInput < 0f && !(_handTransform.transform.position.y < _handBottomBounds)) 
				{
					_wantedTransform.y = _yInput * HandYSensitivity * Time.deltaTime;
				}
				//If we're at the Top or Bottom bounds we rotate the camera instead of moving the hand.
				else if (_yInput != 0f && !_camUpdatedThisFrame)
				{
					_myPlayerCamera.UpdateFreeCamera();
				}
				//Translate the hand by _wanted transform.
				_handTransform.transform.Translate(_wantedTransform);
				//Reset _wanted transform so we don't drift every frame.
				_wantedTransform = Vector3.zero;
				
				//Move the grabbing hand to the UI hand's position.
				//Ray from hand out into space.
				Ray GrabbingHandRay = Camera.main.ScreenPointToRay(_handTransform.transform.position);
				
				//Vector3 of where the grabbing hand should be based on original grabbing distance.
				Vector3 handTransform = GrabbingHandRay.GetPoint (_grabbedDistance);
				_currentInstatiatedHand.transform.position = handTransform;
				
				
				//Store velocity of the grabbed object.
				_grabbedVelocity = (_CurrentGrabbedObjectConfigurableJoint.gameObject.transform.position - _grabbedObjectPositionLastFrame) / Time.deltaTime;
				_grabbedObjectPositionLastFrame = _CurrentGrabbedObjectConfigurableJoint.gameObject.transform.position;
				yield return null;
			}

			//If the joint breaks (Probably because the player tried forcing it through the environment too much)
			if(_CurrentGrabbedObjectConfigurableJoint == null)
			{				
				
				//Destroy the instantiaged hand object.
				Destroy(_currentInstatiatedHand);
				
				//Move the hand to its start.
				_handTransform.DOMove (_startingHandTransform, HandRecenterTime);
				//Set the image to an open hand
				_myImageScript.sprite = OpenHand;
				//Set state back to Free
				CurrentState = State.Free;
				
				
			}
			else
			{
				//Get the game object we grab
				GameObject grabbedObject = _CurrentGrabbedObjectConfigurableJoint.gameObject;
				
				//Destroy the configurabe joint
				Destroy(_CurrentGrabbedObjectConfigurableJoint);
				
				//Destroy the instantiaged hand object.
				Destroy(_currentInstatiatedHand);
				
				//Apply a force to the game object's rigidbody we grabbed equal to its velocity.
				grabbedObject.GetComponent<Rigidbody>().velocity = _grabbedVelocity;
				
				
				//Move the hand to its start.
				_handTransform.DOMove (_startingHandTransform, HandRecenterTime);
				//Set the image to an open hand
				_myImageScript.sprite = OpenHand;
				//Set state back to Free
				CurrentState = State.Free;
			}
		}

		IEnumerator GrabPushable(RaycastHit GrabInfo)
		{
			_myImageScript.sprite = ClosedHand;
		
			//The state is grabbingpushable
			CurrentState = State.GrabbingPushable;



			//Get the grabbed box info.
			_grabbedBox = GrabInfo.transform.gameObject;


			_myImageScript.sprite = ClosedHand;

			CharacterController	_playerCharacterController = _player.GetComponent<CharacterController>();
			Rigidbody _grabbedBoxBody = _grabbedBox.GetComponent<Rigidbody>();


			while (_leftMouse && _equiped && Vector3.Distance(_grabbedBox.transform.position, _player.transform.position) <= PushingBreakDistance)
			{
				_grabbedBoxBody.velocity = _playerCharacterController.velocity;

				yield return null;
			}

			//Hand image back to open.
			_myImageScript.sprite = OpenHand;

			//Set the state to Free
			CurrentState = State.Free;
		}

		IEnumerator GrabDoor(RaycastHit GrabInfo, bool Right)
		{
			//This handles which side we grabbed the door from.
			float AxisMod = Flip ? -1 : 1;
			if (!Right) // Left
			{
				AxisMod = -AxisMod;
			}
			
			//Grabbingdoor is true
			GrabbingDoor = true;
			
			//Set the states.
			CurrentState = State.GrabbingDoor;
			JointMotor motor = _grabbedDoor.motor;
			motor.freeSpin = false;
			
			//Use the door's motor to move it.
			_grabbedDoor.useMotor = true;
			
			//Set the image
			_myImageScript.sprite = DoorHand;
			transform.localScale = _startScale * SmallScaleIncrease;
			
			while (_leftMouse && _equiped)
			{
				
				motor.targetVelocity = DoorTargetVelocity;
				if (_yInput < 0F)
				{
					_grabbedDoor.axis = new Vector3(0f, -1f * AxisMod, 0f);
				}
				else
				{
					_grabbedDoor.axis = new Vector3(0f, 1f * AxisMod, 0f);
					
				}
				
				motor.force = Mathf.Abs(_yInput * DoorForce);
				
				_grabbedDoor.motor = motor;
				
				yield return null;
			}
			CurrentState = State.Free;
			//Stop using the motor.
			_grabbedDoor.useMotor = false;
			
			//Re-open the hand
			_myImageScript.sprite = OpenHand;
			transform.localScale = _startScale;
			
			//Grabbingdoor is false
			GrabbingDoor = false;
		}

		void GrabShelf (RaycastHit GrabInfo)
		{

			//Get the configurable joint
			ConfigurableJoint ShelfJoint = GrabInfo.transform.gameObject.GetComponent<ConfigurableJoint>();

			//If the target position is 0 make it -1 if it's -1 make it 0.
			if(ShelfJoint.targetPosition.z == 0f)
			{
				ShelfJoint.targetPosition = new Vector3(0f,0f, -1f);
			}
			else if(ShelfJoint.targetPosition.z == -1f)
			{
				ShelfJoint.targetPosition = new Vector3(0f,0f,0f);
			}
			//nudge the rigidbody.
			GrabInfo.transform.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0f,0f,.1f));
		}
		#endregion
		
		//How we treat input when we have grabbed something.

		
		//Set where on the screen the cursor hand can go.
		void setHandBounds()
		{
			//Screen height times a percentage.
			_handTopBounds = Screen.height - (Screen.height * OuterRectY);
			_handBottomBounds = (Screen.height * OuterRectY);
			
			_handRightBounds = Screen.width - (Screen.width * OuterRectX);
			_handLeftBounds = Screen.width * OuterRectX;
		}
		
		//Used by equipeditemhandler script to set if the hand is active or not.
		public void setEquipped( bool IsEquipped)
		{
			_equiped = IsEquipped;
			if(_myImageScript == null)
				_myImageScript = GetComponent<Image>();
			if (_equiped == false) 
			{
				_myImageScript.sprite = GunReticle;
			}
			else
			{
				_myImageScript.sprite = OpenHand;
			}
		}
		
	}
}