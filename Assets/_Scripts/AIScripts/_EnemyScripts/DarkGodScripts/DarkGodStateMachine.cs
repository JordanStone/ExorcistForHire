using UnityEngine;
using System.Collections;
using NashTools;

namespace BossDetectionScript
{
	public class DarkGodStateMachine : MonoBehaviour {
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


		public enum BossState 
		{
			Patrol,
			Attack,
			Search,
			Heal,
			Die
		}

		public enum BossPhase
		{
			One,
			Two,
			Three
		}

		private bool _changingState = false;
		private BossState _lastState;

		public BossState currentState = BossState.Patrol;
		public BossPhase currentPhase = BossPhase.One;

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

		[Header("How fast I move when I'm just walking around normally.")]
		public float PathSpeed = 2f;
		[Header("How fast I move when I'm chasing the player")]
		public float ChaseSpeed = 6.5f;
		
		[Header("How fast I move when I'm looking for the player")]
	
		public float SearchSpeed = 4f;



		private NavMeshAgent _myNavMeshAgent;

		[Header("How long do I stay in search before givinh up?")]
		public float SearchTime = 10f;

		//Our vision cone angle
		public float fieldOfViewAngle = 110f;

		[Header("Don't change this.")]
		//Is the player visable?
		public bool _playerVisable;

		[Header("How far away can I see the player from?")]
		public float SightDistance = 30f;

		[Header("the player, I can find this myself, don't bother setting it.")]
		//The player transform.
		public Transform _player;

		[Header("What layers do my raycasts HIT? What CAN the player hide behind.")]
		public LayerMask SearchingIgnoreLayer;
		private Vector3 _lastKnownPosition;
		private Vector3 _fountainPosition;

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
		public FollowBossPath PatrolPath;

		private BossBehavior _bossBehaviorScript;

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

		[SerializeField]
		private int totalBossAdds;




		void Start () 
		{
			_myAnimator = GetComponent<Animator>();
			_myNavMeshAgent = GetComponent<NavMeshAgent>();
			_player = GameObject.Find("Player").transform;
			_searchPath = new NavMeshPath ();
			_bossBehaviorScript = gameObject.GetComponentInParent<BossBehavior>();
			_fountainPosition = GameObject.Find("Fountain").transform.position;


			if(MyZone != null)
			{
				_interestAreas = MyZone.GetComponentsInChildren<SphereCollider>();
			}

			totalBossAdds = GameObject.FindGameObjectsWithTag("BossAdd").Length;


		}
		void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(searchLocation, 1);
		}

		void Update () 
		{

			if(!_changingState)
			{
				if (currentState == BossState.Attack) 
				{
					UpdateAttack();
				}
				if (currentState == BossState.Patrol)
				{
					UpdatePatroll();
				}
				if(currentState == BossState.Search)
				{
					UpdateSearch();
				}
				if(currentState == BossState.Die)
				{
					UpdateDie();
				}
				if(currentState == BossState.Heal)
				{
					UpdateHeal();
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
				if( hitAngle < fieldOfViewAngle *.5f && Vector3.Distance(_player.transform.position, transform.position) < SightDistance) 
				{
					 _playerVisable = true;	
				}
				else
				{
					_playerVisable = false;
				}


			}

			//	Debug.DrawLine(transform.position,_player.position + new Vector3(0f, 1.5f, 0f), Color.yellow, .5f);

			_myNavMeshAgent.speed = Speed;
		}


		public IEnumerator ChangeState(BossState STATE)
		{
			_changingState = true;
			switch(currentState)
			{
			case BossState.Attack:
				yield return StartCoroutine(ExitAttack());
				break;
			case BossState.Search:
				yield return StartCoroutine(ExitSearch());
				break;
			case BossState.Patrol:
				yield return StartCoroutine(ExitPatroll());
				break;
			case BossState.Die:
				yield return StartCoroutine(ExitDie());
				break;
			case BossState.Heal:
				yield return StartCoroutine(ExitHeal());
				break;
			}

			switch(STATE)
			{
			case BossState.Attack:
				yield return StartCoroutine(EnterAttack());
				break;
			case BossState.Search:
				yield return StartCoroutine(EnterSearch());
				break;
			case BossState.Patrol:
				yield return StartCoroutine(EnterPatroll());
				break;
			case BossState.Die:
				yield return StartCoroutine(EnterDie());
				break;
			case BossState.Heal:
				yield return StartCoroutine(EnterHeal());
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

				StartCoroutine(ChangeState(BossState.Attack));
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
			if(_lastState != BossState.Attack)
			{
				//_myAnimator.SetTrigger("Alert");
				//while(_myAnimator.GetFloat("AlertUsed") <= .9f)
				//{
				//	Speed = 0f;
				//	yield return null;
				//}
			}
			_loseTimer = LoseTime;
			yield return null;
		}

		void UpdateAttack()
		{

			if(_playerVisable == false )
			{
				_loseTimer -= Time.deltaTime;

				if(_loseTimer <= 0f)
				{
					StartCoroutine(ChangeState(BossState.Search));
				}
			}
			else if(_bossBehaviorScript.inAttack == true)
			{
				Speed = 0f;
			}
			else
			{
				Speed = ChaseSpeed;
				_myAnimator.SetBool("IsWalking", true);
				PatrolPath.Attack();
				_loseTimer = LoseTime;

			}
		}



		IEnumerator ExitAttack()
		{
			if(!_myAnimator.GetBool("IsWalking"))
			{	//Wait for attack animation to finish while attacking.
			/*	while(_myAnimator.GetFloat("AttackUsed") <= .6f)
				{
					Speed = 0f;
					yield return null;
				} */
				Speed = 0f;
			}
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
	
			Speed = SearchSpeed;

			_myNavMeshAgent.SetDestination(searchLocation);

			if (_playerVisable) {
				StartCoroutine (ChangeState (BossState.Attack));
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
				StartCoroutine(ChangeState(BossState.Patrol));
			}
		}

		IEnumerator ExitSearch()
		{			
			yield return null;
		}

		#endregion


		#region Heal
		IEnumerator EnterHeal()
		{
			//Debug.Log("currentState = EnterHeal");
			yield return null;
		}

		void UpdateHeal()
		{
			//Debug.Log("CurrentState = UpdateHeal");
			searchLocation = _fountainPosition;
			_myNavMeshAgent.SetDestination(searchLocation);

			if(_hasSearchDestination == false)
			{
				NavMesh.CalculatePath(transform.position, searchLocation , NavMesh.AllAreas, _searchPath);
			}
			

			if(_searchPath.status == NavMeshPathStatus.PathComplete)
			{
				//Debug.Log("path is complete");
				_myNavMeshAgent.SetPath(_searchPath);
				_hasSearchDestination = true;
			}
			if(_hasSearchDestination == true)
			{
				Speed = ChaseSpeed;
				//Debug.Log("_hasSearchDestination is true");
				transform.LookAt(_fountainPosition);
				//Debug.Log("Vector3 Distance: " + Vector3.Distance(transform.position, searchLocation));
				//Debug.Log("Target position (" + searchLocation.x + ", " + searchLocation.y + ", " + searchLocation.z);
				if(Vector3.Distance(transform.position, searchLocation) < 2f)
				{
					_hasSearchDestination = false;
					_atSearchDestination = true;
					//Debug.Log("Commencing healing");
					_bossBehaviorScript.HealHealth();
					Speed = 0f;
					transform.LookAt(_lastKnownPosition);

				}
				else
				{
					_atSearchDestination = false;
				}
			}
		}

		IEnumerator ExitHeal()
		{
			_bossBehaviorScript.StopHealing();
			yield return null;
		}

		#endregion

		#region Die
		IEnumerator EnterDie()
		{
			Speed = 0f;

			yield return new WaitForSeconds(5);
			Destroy(gameObject);
		}
		void UpdateDie()
		{
			
			//if(_myAnimator.GetFloat("DeathUsed") == 1f)
			//{

				Destroy(gameObject);
			//}
		}

		IEnumerator ExitDie()
		{

			yield return null;

		}

		#endregion

		public void OnBossAddDeath()
		{
			totalBossAdds -= 1;
			if(totalBossAdds <= 0)
			{
				currentPhase = BossPhase.Two;
				_bossBehaviorScript.AddPhaseAttacks(currentPhase);
				GameObject shield = transform.Find("Shield").gameObject;
				if(shield != null)
					shield.SetActive(false);


			}
		}

		public void OnHealingObjectDeath()
		{
			currentPhase = BossPhase.Three;
			_bossBehaviorScript.AddPhaseAttacks(currentPhase);
			StartCoroutine(ChangeState(BossState.Search));
		}

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
	}
}
