using UnityEngine;
using System.Collections;
using NashTools;
using DG.Tweening;

namespace EnemyDetectionScript
{
	public class EnemyStateMachine: MonoBehaviour {
		private float hitAngle;


		public enum EnemyMovementType
		{
			FollowPath,
			WaitForTrigger,
			ZoneBased
		}

		public EnemyMovementType MovementType;

		public enum EnemyAttackType
		{
			Ranged,
			Melee
		}

		public EnemyAttackType AttackType;


		public enum EnemyState 
		{
			Patrol,
			Attack,
			Search,
			Die
		}
		private bool _changingState = false;
		private EnemyState _lastState;

		public EnemyState currentState = EnemyState.Patrol;

		[Header("How long do I chase the player for after I lose sight of them before entering search?")]
		public float ChaseTime = 5f;
		[Header("Time between attacks until I can attack again.")]
		public float WaitTimeAfterAttack = 5f;
		private float _waitTimer;

		[Header("How fast I move after attacking")]
		public float HideSpeed = 2f;

		private Animator _myAnimator;
		static int DeathState = Animator.StringToHash("Base Layer.Death");

		public float Speed;

		private float _lastAngle;
		private float _refAngle;

		[Header("How fast I move when I'm just walking around normally.")]
		public float PathSpeed = 2f;
		[Header("How fast I move when I'm chasing the player")]
		public float ChaseSpeed = 6.5f;

		[Header("Odds Of Dodging when the player is looking at me.")]
		//Odds  X/100 of calling a dodge at any given second. 
		public float DodgeChance = 7f;
		private float _dodgeTimer;
		private float _dodgePecent;
		private int DodgeDirection;
		
		[Header("How fast I move when I'm looking for the player")]
	
		public float SearchSpeed = 4f;



		private NavMeshAgent _myNavMeshAgent;

		[Header("How long do I stay in search before giving up?")]
		public float SearchTime = 10f;

		//Our vision cone angle
		public float fieldOfViewAngle = 110f;
		private float _currentfieldOfViewAngle;
		public float flashlightfieldOfViewAngle = 360f;

		[Header("Don't change this.")]
		//Is the player visable?
		public bool _playerVisable;

		[Header("How far away can I see the player from?")]
		public float SightDistance = 15f;
		public float flashlightOnSightDistance = 30f;
		private float DefaultSightDistance;
		private Light _PlayerFlashlight;
		
		[Header("the player, I can find this myself, don't bother setting it.")]
		//The player transform.
		public Transform _player;

		[Header("What layers do my raycasts HIT? What CAN the player hide behind.")]
		public LayerMask SearchingIgnoreLayer;
		private Vector3 _lastKnownPosition;

		[Header("How Long Until We lose the player once we lose sight of them")]
		public float LoseTime = 1f;
		private float _loseTimer;

		[Header("Max Radius to look for the player")]
		public float SearchRadius = 8f;
		private NavMeshPath _searchPath;
		private Vector2 circleLocation;
		private Vector3 searchLocation;
		private bool _atSearchDestination = false;
		private bool _hasSearchDestination = false;


		//FollowPath Variables
		public FollowPath PatrolPath;

		//WaitForPlayer Variables
		private bool _playerInTrigger;

		[Header("Variables for Zone Based Movement")]
		//ZoneBased Variables
		[Header("An empty gameobject with child objects that have sphere triggers in them")]
		public GameObject MyZone;
		public float ZoneStayTime = 10f;
		public float ZoneStayTimeVariance = 5f;
		private float _zoneTimer;
		private SphereCollider[] _interestAreas;
		private SphereCollider _currentTargetZone;
		private Vector3 _currentTargetPositionInZone;
		private bool _hasTargetInZone;

		[Header("Ranged Attack Variables")]
		public GameObject Projectile;
		private Rigidbody ProjectileRigidbody;
		public float ProjectileAttackRange = 40f;
		public float PVelocity = 10f;
		public float PAngle;
		private float PTimer;
		public float Ptime = 3f;
		public Transform HandJoint;

		private Vector3 _startingPosition;






		void Awake () 
		{
			_startingPosition = transform.position;
			DefaultSightDistance = SightDistance;
		}

		void OnEnable() 
		{
			_myAnimator = GetComponent<Animator>();
			_myNavMeshAgent = GetComponent<NavMeshAgent>();
			_player = GameObject.Find("Player").transform;
			_searchPath = new NavMeshPath ();

			if(MyZone != null)
			{
				_interestAreas = MyZone.GetComponentsInChildren<SphereCollider>();
			}

			ProjectileRigidbody = Projectile.GetComponent<Rigidbody>();
			PTimer = 0f;

			transform.position = _startingPosition;

			currentState = EnemyState.Patrol;
			_lastState = EnemyState.Patrol;
			_changingState = false;
			_waitTimer = 0f;

			_PlayerFlashlight = GameObject.Find("Flashlight").GetComponent<Light>();

			_currentfieldOfViewAngle = fieldOfViewAngle;
			

		}
		void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(searchLocation, 1);
		}

		void Update () 
		{
			_myNavMeshAgent.speed = Speed;

			if(!_changingState)
			{
				if (currentState == EnemyState.Attack) 
				{
					UpdateAttack();
				}
				if (currentState == EnemyState.Patrol)
				{
					UpdatePatroll();
				}
				if(currentState == EnemyState.Search)
				{
					UpdateSearch();
				}
				if(currentState == EnemyState.Die)
				{
					UpdateDie();
				}
			}

			RaycastHit Hit;
			//Keep track of if the enemy can see the player.
			if(Physics.Linecast(transform.position, _player.position + new Vector3(0f, 1.5f, 0f), out Hit, SearchingIgnoreLayer))
			{
				//Debug.Log (Hit.transform.gameObject.name);

				_playerVisable = false;
			}
			else
			{
				//Now we compare angles.
				hitAngle = Vector3.Angle(_player.transform.position - transform.position, transform.forward);
				//If we hit the current at the wrong angle. 
				if( hitAngle < _currentfieldOfViewAngle *.5f && Vector3.Distance(_player.transform.position, transform.position) < SightDistance) 
				{
					 _playerVisable = true;	
				}
				else
				{
					_playerVisable = false;
				}


			}


			//This is where I set the speeds for Grounded.

			//The fastest I move is my chaseSpeed. So I make 0% not moving and max chase speed 100%

	
			//Compare angle we were at to angle we're changing to and smooth it and devide by 4 to get a value between 0 and 1 for how much we turn.
			float angle = Vector3.Angle(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
			Vector3 cross = Vector3.Cross(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
			if(cross.y < 0) angle = -angle;
			if(angle > 7f || angle < -7f) angle = 0f;


			angle = Mathf.SmoothDamp(_lastAngle, angle, ref _refAngle, .5f);

			//Debug.Log(Vector3.Magnitude(_myNavMeshAgent.velocity).ToString());
			_myAnimator.SetFloat("Forward", Speed/ChaseSpeed);
			_myAnimator.SetFloat("Turn", angle/4f);
		
			_lastAngle = angle;


			//If we're hit no matter what state we're in speed is 0.
			if(_myAnimator.GetCurrentAnimatorStateInfo(0).IsName("HumanHit_Stomach")|| 
			   _myAnimator.GetCurrentAnimatorStateInfo(0).IsName("HumanHit_Face") ||
			   _myAnimator.GetCurrentAnimatorStateInfo(0).IsName("HumanHit_Chest"))
			{
				_myNavMeshAgent.speed = 0f;
			}
		}


		public IEnumerator ChangeState(EnemyState STATE)
		{
			_changingState = true;
			switch(currentState)
			{
			case EnemyState.Attack:
				yield return StartCoroutine(ExitAttack());
				break;
			case EnemyState.Search:
				yield return StartCoroutine(ExitSearch());
				break;
			case EnemyState.Patrol:
				yield return StartCoroutine(ExitPatroll());
				break;
			case EnemyState.Die:
				yield return StartCoroutine(ExitDie());
				break;
			}

			switch(STATE)
			{
			case EnemyState.Attack:
				yield return StartCoroutine(EnterAttack());
				break;
			case EnemyState.Search:
				yield return StartCoroutine(EnterSearch());
				break;
			case EnemyState.Patrol:
				yield return StartCoroutine(EnterPatroll());
				break;
			case EnemyState.Die:
				yield return StartCoroutine(EnterDie());
				break;
			}	
			// Handleing _lastState is a bit strange.
			_lastState = currentState;

			currentState = STATE;
			yield return null;
			_changingState = false;
		}
		#region PATROLL

		IEnumerator EnterPatroll()
		{
			//If we already did the trigger event once and the player escaped us, start doing the normal follow path.
			if(MovementType == EnemyMovementType.WaitForTrigger && _playerInTrigger)
			{
				MovementType = EnemyMovementType.FollowPath;
			}

			yield return null;
		}

		void UpdatePatroll()
		{ 	
			setSight();
			//For followpath enemies
			if(MovementType == EnemyMovementType.FollowPath)
			{
				Speed = PathSpeed;
				PatrolPath.Patrol();
			}
			//For Wait for trigger enemies
			else if(MovementType == EnemyMovementType.WaitForTrigger)
			{
				//when the player enters the trigger attack them.
				if(_playerInTrigger == true)
				{
					_myNavMeshAgent.SetDestination(_player.position);
					Speed = ChaseSpeed;
				}
				//if the player isn't in the trigger don't chase them.
				else
				{
					Speed = 0f;
				}

			}
			//for ZoneBased
			else if(MovementType == EnemyMovementType.ZoneBased)
			{			
				if(_hasTargetInZone == false || _zoneTimer <= 0f)
				{
					_currentTargetZone = _interestAreas[Random.Range(0, _interestAreas.Length)];
					_currentTargetPositionInZone = _currentTargetZone.transform.position + Vector3.ClampMagnitude( new Vector3(Random.Range(0,360), 0f, Random.Range(0,360)), Random.Range(0f, _currentTargetZone.radius));
					//Need to add random variation inside circle.
					_myNavMeshAgent.SetDestination(_currentTargetPositionInZone);
									
					_hasTargetInZone = true;
					_zoneTimer = ZoneStayTime + Random.Range(-ZoneStayTimeVariance , ZoneStayTimeVariance);

					Speed = PathSpeed;
				}
				else if(Vector3.Distance(transform.position, _currentTargetPositionInZone) < 1f)
				{
					Speed = 0f;

					_zoneTimer -= Time.deltaTime;
				
					if(_zoneTimer <= 0f)
					{
						_hasTargetInZone = false;
					}

				}
				                                
			}

			if(_playerVisable)
			{
				_zoneTimer = 0f;
				_hasTargetInZone = false;

				StartCoroutine(ChangeState(EnemyState.Attack));
			}
		}

		IEnumerator ExitPatroll()
		{
			if(MovementType == EnemyMovementType.WaitForTrigger)
			{
				_playerInTrigger = true;
			}

			yield return null;
		}
		#endregion


		//Attack includes chase.
		#region ATTACK
		IEnumerator EnterAttack()
		{
			//If last state was attack don't play alert again
			if(_lastState != EnemyState.Attack)
			{

			
			}
			_loseTimer = LoseTime;
			_dodgeTimer = 1f;

			yield return null;
		}

		void UpdateAttack()
		{
			_myAnimator.SetBool("Attack", false);
			//if im close to the player. And if my current state is called grounded.
			if( Vector3.Distance(transform.position, _player.position) <= 2f && _myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				Speed = 2f;
				_myAnimator.SetBool("Attack", true);
			}
			else if(!_myAnimator.IsInTransition(0) && !_myAnimator.GetCurrentAnimatorStateInfo(0).IsName("HumanAttack_Axe"))
			{

				if(_playerVisable == false )
				{
					_loseTimer -= Time.deltaTime;

					if(_loseTimer <= 0f)
					{
						StartCoroutine(ChangeState(EnemyState.Search));
					}
				}
				else
				{
					Speed = ChaseSpeed;
				
					_loseTimer = LoseTime;

					if(AttackType == EnemyAttackType.Ranged)
					{
						float dTop = Vector3.Distance(transform.position, _player.position); 
						
						if( dTop <= ProjectileAttackRange)
						{
							Speed = 0f;

							if( !Projectile.activeSelf)
							{
						
								Projectile.SetActive(true);
								Projectile.transform.position = HandJoint.position;

								Projectile.transform.LookAt(_player.position);

								Projectile.transform.eulerAngles = new Vector3(GetAttackArk(_player.transform.position) + Projectile.transform.eulerAngles.x, Projectile.transform.eulerAngles.y, Projectile.transform.eulerAngles.z);

								ProjectileRigidbody.velocity = Vector3.zero;
								ProjectileRigidbody.AddForce(Projectile.transform.forward * PVelocity, ForceMode.VelocityChange);

								PTimer = Ptime;
							}
							else
							{
								PTimer -= Time.deltaTime;
								if(PTimer <= 0f)
								{
									Projectile.SetActive(false);
								}
							}


						}
					}

					//Are we looking at the player and are they looking at us because I want sempai to notice me.
					float hitAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, _player.transform.rotation.eulerAngles.y);

					//This returns around 153 to 175 we should probably to abs of hitAngle for this as well.
					hitAngle = Mathf.Abs(hitAngle);

					if( hitAngle > 153f &&  hitAngle < 175f && _dodgeTimer > 0f)
					{
						_dodgeTimer -= Time.deltaTime;

						DodgeDirection = Random.Range(1,5);
						_dodgePecent = Random.Range(0,100);
					}
					if(_dodgeTimer <= 0f && DodgeChance >= _dodgePecent && !_myAnimator.IsInTransition(0))
					{
						float dodgeSpeed = 10f;

						Speed = 0f;

						switch(DodgeDirection)
						{				
						case 1:
							transform.position += transform.forward * Time.deltaTime * dodgeSpeed;
							_myAnimator.SetTrigger("RollForward");
							break;
						case 2:
							transform.position += transform.right * Time.deltaTime * dodgeSpeed;
							_myAnimator.SetTrigger("RollRight");
							break;
						case 3:
							transform.position += -transform.forward * Time.deltaTime * dodgeSpeed;
							_myAnimator.SetTrigger("RollBackward");
							break;
						case 4:
							transform.position += -transform.right * Time.deltaTime * dodgeSpeed;
							_myAnimator.SetTrigger("RollLeft");
							break;
						}

						_dodgeTimer -= Time.deltaTime;
					}
					else if (_dodgeTimer <= 0f && DodgeChance < _dodgePecent )
					{	
						_dodgeTimer = 1f;
					}
					if(_dodgeTimer <= -.2f)
					{
						_dodgeTimer = 1f;
					}
				}
			}
			else
			{
				Speed = 0f;
			}
		}



		IEnumerator ExitAttack()
		{
		
			yield return null;
		}

		#endregion

		#region Search

		IEnumerator EnterSearch()
		{
			_lastKnownPosition = _player.position;
			searchLocation = _lastKnownPosition;

			//Set the searchtimer.
			_waitTimer = SearchTime;
			_hasSearchDestination = false;

			yield return null;
		}


		void UpdateSearch()
		{
			setSight();
			Speed = SearchSpeed;

			_myNavMeshAgent.SetDestination(searchLocation);

			if (_playerVisable) {
				StartCoroutine (ChangeState (EnemyState.Attack));
			}
			else if(_hasSearchDestination == false)
			{
				circleLocation = Random.insideUnitCircle * SearchRadius;
				searchLocation = new Vector3(circleLocation.x + _player.transform.position.x, _player.transform.position.y, circleLocation.y + _player.transform.position.z);

				NavMesh.CalculatePath(transform.position, searchLocation , NavMesh.AllAreas, _searchPath);

				if(_searchPath.status == NavMeshPathStatus.PathComplete)
				{
					_myNavMeshAgent.SetPath(_searchPath);
					_hasSearchDestination = true;
				}
			}
			else if(_hasSearchDestination == true)
			{
				if(Vector3.Distance(transform.position, searchLocation) < 3f)
				{
					_hasSearchDestination = false;
					_atSearchDestination = true;
				}
				else
				{
					_atSearchDestination = false;
				}

			}



			_waitTimer -= Time.deltaTime;

			if (_waitTimer <= 0f)
			{
				StartCoroutine(ChangeState(EnemyState.Patrol));
			}
		}

		IEnumerator ExitSearch()
		{			
			yield return null;
		}

		#endregion


		#region Die
		IEnumerator EnterDie()
		{
			Speed = 0f;

			yield return new WaitForSeconds(5);

			gameObject.SetActive(false);
		}


		void UpdateDie()
		{
		
		}

		IEnumerator ExitDie()
		{
	
			yield return null;

		}

		#endregion



		void OnTriggerEnter(Collider c) 
		{

		}

		void OnTriggerStay(Collider c) 
		{

		}

		public void SetPlayerInTrigger(bool Triggered)
		{
			_playerInTrigger = Triggered;

		}

		public float GetAttackArk(Vector3 Target)
		{
			Vector2 P;

			//Distance no Y;
			float TD = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(Target.x, 0f, Target.z));
			float TY = transform.position.y - Target.y;

			P = new Vector2(TD, TY);


			//Need to set enemy to 0,0 and player to X,Y
			//Somehow? Get distance to player minus Height, then height distance?
				//Should work.

			//The angle we return
			float Theta;
			//Gravity.
			float g = Physics.gravity.y;
			//The launch velocity.
			float v = PVelocity;
			//X of the Target Position... Distance from player ignoring height.
			float x = P.x;
			//Y of the target position... Height difference from player.
			float y = P.y;

			Theta = Mathf.Atan2
			(
				Mathf.Pow(v, 2f) + Mathf.Sqrt(Mathf.Pow(v,4f) - g * (g * Mathf.Pow(x, 2f) + 2 * y * Mathf.Pow(v, 2f))),	g*x
			
			);


			return (Mathf.Rad2Deg * Theta);

		}



		void setSight()
		{
			//
			if(_PlayerFlashlight.enabled)
			{
				SightDistance = flashlightOnSightDistance;
				_currentfieldOfViewAngle = flashlightfieldOfViewAngle;
			}
			else
			{
				SightDistance = DefaultSightDistance;
				_currentfieldOfViewAngle = fieldOfViewAngle;
			}
		}

				/*
		 * g: the gravitational acceleration—usually taken to be 9.81 m/s2 near the Earth's surface
			θ: the angle at which the projectile is launched
			v: the speed at which the projectile is launched
			y0: the initial height of the projectile
			d: the total horizontal distance traveled by the projectile
		 * 
		 *
		 */
	/*	IEnumerator ThrowProjectile(float dTop)
		{
			Projectile.SetActive(true);
			Projectile.transform.position = HandJoint.position;

			Tween myTween = Projectile.transform.DOJump(_player.transform.position + Vector3.up, dTop * PHeightMod, 1, dTop * PTimeMod).SetEase(Ease.Linear);
			yield return myTween.WaitForCompletion();

			Projectile.SetActive(false);
		}
		*/
	}
}