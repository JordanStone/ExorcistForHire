using UnityEngine;
using System.Collections;
using SoundController;
using DG.Tweening;
using NashTools;

[RequireComponent (typeof (CharacterController))]

public class FPSController: MonoBehaviour {
	
	public float walkSpeed = 6.0f;
	
	public float runSpeed = 11.0f;

	public float crouchSpeed = 3.0f;
	
	// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
	public bool limitDiagonalSpeed = true;
	
	// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	// There must be a button set up in the Input Manager called "Run"
	public bool toggleRun = false;
	
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	
	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;
	
	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	public bool slideWhenOverSlopeLimit = false;
	
	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;
	
	public float slideSpeed = 12.0f;
	
	// If checked, then the player can change direction while in the air
	public bool airControl = false;
	
	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;
	
	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 1;
	
	// Audio Variables
	public AudioSource walkSound;

	private AudioClip grassWalk;
	private AudioClip woodWalk;
	private string currentSurface;
	private SoundManager _soundManager;
	public float minPitch = 0.7f;
	public float maxPitch = 1.4f;
	
	private Vector3 moveDirection = Vector3.zero;
	private bool grounded = false;
	private CharacterController controller;
	private Transform myTransform;
	private float speed;
	private RaycastHit hit;
	private float fallStartLevel;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private bool crouched = false;
	private bool canStand = true;
	private int jumpTimer;

	// Crouch Variables
	private Vector3 standardScale;
	public float crouchDifference = 0.5f;
	private float childCrouchY;
	public float crouchTweenTime = 0.2f;

	// Ladder Variables
		//Is movement locked?
	private bool _locked = false;


	void Start() {
		DOTween.Init();
		controller = GetComponent<CharacterController>();
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f; //+ controller.radius
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;

		standardScale = myTransform.localScale;
		childCrouchY = 1 / crouchDifference;

		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		//AudioSource[] audioSources = GetComponents<AudioSource>();
		grassWalk = _soundManager.GetWalkSound(0);
		woodWalk =  _soundManager.GetWalkSound(1);
		
		currentSurface = "Terrain";
		walkSound.clip = grassWalk;


		//For Ladde
		EventManager.AddListener((int) GameManagerScript.GameEvents.Ladder, OnLadder);
	}
	
	void FixedUpdate() 
	{	//We cant move while grabbing a door.
		if (MouseController.Mouse_Controller.GrabbingDoor == false) 
		{
			//We can't move while on a ladder.
			if(!_locked)
			{
				float inputX = Input.GetAxis ("Horizontal");
				float inputY = Input.GetAxis ("Vertical");
				// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
				float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;
				
				if (grounded) {
					bool sliding = false;
					// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
					// because that interferes with step climbing amongst other annoyances
					if (Physics.Raycast (myTransform.position, -Vector3.up, out hit, rayDistance)) {
						//				print (hit.transform.name);
						if (Vector3.Angle (hit.normal, Vector3.up) > slideLimit)
							sliding = true;				
					}
					// However, just raycasting straight down from the center can fail when on steep slopes
					// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
					else {
						Physics.Raycast (contactPoint + Vector3.up, -Vector3.up, out hit);
						if (Vector3.Angle (hit.normal, Vector3.up) > slideLimit)
							sliding = true;
					}
					
					//print(hit.transform.name);
					//print(currentSurface);
					if (hit.transform.name.Contains("Terrain") && !currentSurface.Contains("Terrain")) {
						walkSound.Stop ();
						walkSound.clip = grassWalk;
						walkSound.Play ();
					} else if (hit.transform.name.Contains ("floor") && !currentSurface.Contains ("floor")) {
						walkSound.Stop ();
						walkSound.clip = woodWalk;
						walkSound.Play ();
					}
					
					currentSurface = hit.transform.name;
					
					// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
					if (falling) {
						falling = false;
						if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
							FallingDamageAlert (fallStartLevel - myTransform.position.y);
					}
					
					// If running isn't on a toggle, then use the appropriate speed depending on whether the crouch button or run button is down. Script written such that player cannot crouch and run at once.
					if (!toggleRun)
						speed = crouched ? crouchSpeed : Input.GetButton ("Run") ? runSpeed : walkSpeed;

					// Change transform scale on crouch
					if (Input.GetButtonDown ("Crouch"))
					{
	//					myTransform.localScale = crouchScale;
						myTransform.DOScaleY(crouchDifference, crouchTweenTime);
						myTransform.GetChild(0).DOScaleY(childCrouchY, crouchTweenTime);
						canStand = false;
						crouched = true;
					}
					else if (Input.GetButtonUp ("Crouch"))
					{
						if(canStand)
						{
							myTransform.DOScaleY(standardScale.y,crouchTweenTime);
							myTransform.GetChild(0).DOScaleY(standardScale.y,crouchTweenTime);
							crouched = false;
						}
						
					}

					
					// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
					if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide")) {
						Vector3 hitNormal = hit.normal;
						moveDirection = new Vector3 (hitNormal.x, -hitNormal.y, hitNormal.z);
						Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
						moveDirection *= slideSpeed;
						playerControl = false;
					}
					// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
					else {
						moveDirection = new Vector3 (inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
						moveDirection = myTransform.TransformDirection (moveDirection) * speed;
						playerControl = true;
					}
					
					// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
					if (!Input.GetButton ("Jump"))
						jumpTimer++;
					else if (jumpTimer >= antiBunnyHopFactor) {
						moveDirection.y = jumpSpeed;
						jumpTimer = 0;
					}

					//If the player is crouched, check to see if there is an object above them. If there is not and crouch button is not pressed,
					//Uncrouch player
					if(crouched)
					{
						RaycastHit hitUp;
						RaycastHit hitBack;
						Vector3 camPos = Camera.main.gameObject.transform.position;
						Vector3 forwardOffset = new Vector3(0f, 0f, 0.5f);

						if (!Physics.Raycast (camPos + forwardOffset, Vector3.up, out hitUp, crouchDifference*4f) && 
							!Physics.Raycast (camPos + -1f* forwardOffset, Vector3.up, out hitBack, crouchDifference*4f))
						{
							canStand = true;
							if(!Input.GetButton("Crouch"))
							{
								myTransform.DOScaleY(standardScale.y, crouchTweenTime);
								myTransform.GetChild(0).DOScaleY(standardScale.y, crouchTweenTime);
								crouched = false;
							}
						}
						else
						{
							canStand = false;
						}
							
					}

				} else {
					// If we stepped over a cliff or something, set the height at which we started falling
					if (!falling) {
						falling = true;
						fallStartLevel = myTransform.position.y;
					}
					
					// If air control is allowed, check movement but don't touch the y component
					if (airControl && playerControl) {
						moveDirection.x = inputX * speed * inputModifyFactor;
						moveDirection.z = inputY * speed * inputModifyFactor;
						moveDirection = myTransform.TransformDirection (moveDirection);
					}
				}
				
				// Apply gravity
				moveDirection.y -= gravity * Time.deltaTime;
				
				// Play Walk Sound

				float pitchLevel = Random.Range(minPitch, maxPitch);
				//print("The pitch level is " + pitchLevel);
				walkSound.pitch = pitchLevel;
				if (grounded && moveDirection.x != 0 && moveDirection.z != 0) {
					// If sound isn't currently playing
					if (!walkSound.isPlaying)
						walkSound.Play ();
				} else {
					walkSound.Stop ();
				}
				
				// Move the controller, and set grounded true or false depending on whether we're standing on something
				grounded = (controller.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
			}
		}
	}
	
	void Update () 
	{
		// If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
		// FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
		if (toggleRun && grounded && Input.GetButtonDown("Run"))
			speed = (speed == walkSpeed? runSpeed : walkSpeed);
	}
	
	// Store point that we're in contact with for use in FixedUpdate if needed
	void OnControllerColliderHit (ControllerColliderHit hit) 
	{
		contactPoint = hit.point;
	
	}
	
	// If falling damage occured, this is the place to do something about it. You can make the player
	// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	void FallingDamageAlert (float fallDistance) 
	{
		//fallDistance;   
	}

	void OnLadder(Component poster, object onLadder)
	{
		_locked = (bool) onLadder;
	}

}