using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcBehavior : MonoBehaviour {

	public enum CurrentBehavior
	{
		StandingIdle,
		LeanOnWall,
		ChopWood,
		Talking,
		PathBased,
		ZoneBased
	};

	public CurrentBehavior myCurrentBehavior;

	//We need this to see which way the char is facing.
	public Transform MyHips;

	private Animator _myAnimator;
	private GameObject _player;
	public PathDefinition Path;
	public float WalkSpeed = 3;

	private bool _talkingDistance;

	public float GoalDistance = .1f;

	private IEnumerator<Transform> currentPoint;
	private NavMeshAgent _myNavMeshAgent;
	private Vector3 _lastTransform;
	private float _lastAngle;
	private float _refAngle;
	private float _refHeadTurn;
	private float _refSpeed;

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

	

	// Use this for initialization
	void Start () 
	{
		_myAnimator = GetComponent<Animator>();

		_myNavMeshAgent = GetComponent<NavMeshAgent>();

		if(Path == null) {
			Debug.LogError("Path cannot be null.", gameObject);
			return;
		}
		currentPoint = Path.GetPathEnumerator();
		currentPoint.MoveNext();
		if(currentPoint.Current == null)
			return;
	
		//transform.position = currentPoint.Current.position;
		_lastTransform = transform.position;

		if(MyZone != null)
		{
			_interestAreas = MyZone.GetComponentsInChildren<SphereCollider>();
		}

		_myNavMeshAgent.speed = 0f;

		_player = GameObject.Find("Player");

		MyHips = transform.Find("master/Reference/Hips");

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!_talkingDistance)
		{
			if(myCurrentBehavior == CurrentBehavior.LeanOnWall)
			{
				_myAnimator.SetBool("IdleOnWall", true);
				_myNavMeshAgent.speed = 0f;

			}
			else
			{
				_myAnimator.SetBool("IdleOnWall", false);
			}

			if(myCurrentBehavior == CurrentBehavior.ChopWood)
			{
				_myAnimator.SetBool("ChoppingWood", true);
				_myNavMeshAgent.speed = 0f;
			}
			else
			{
				_myAnimator.SetBool("ChoppingWood", false);
			}
			if(myCurrentBehavior == CurrentBehavior.Talking)
			{
				_myAnimator.SetBool("Talking", true);
			}
			else
			{
				_myAnimator.SetBool("Talking", false);
			}

			if(myCurrentBehavior == CurrentBehavior.ZoneBased)
			{
				//Compare angle we were at to angle we're changing to and smooth it and devide by 4 to get a value between 0 and 1 for how much we turn.
				float angle = Vector3.Angle(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
				Vector3 cross = Vector3.Cross(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
				if(cross.y < 0) angle = -angle;
				
				
				angle = Mathf.SmoothDamp(_lastAngle, angle, ref _refAngle, .5f);
				
				//Debug.Log(Vector3.Magnitude(_myNavMeshAgent.velocity).ToString());
				_myAnimator.SetFloat("Forward", _myNavMeshAgent.speed/(WalkSpeed*1.5f));
				_myAnimator.SetFloat("Turn", angle/4f);

				if(_hasTargetInZone == false || _zoneTimer <= 0f)
				{
					_currentTargetZone = _interestAreas[Random.Range(0, _interestAreas.Length)];
					_currentTargetPositionInZone = _currentTargetZone.transform.position + Vector3.ClampMagnitude( new Vector3(Random.Range(0,360), 0f, Random.Range(0,360)), Random.Range(0f, _currentTargetZone.radius));
					//Need to add random variation inside circle.
					_myNavMeshAgent.SetDestination(_currentTargetPositionInZone);
					
					_hasTargetInZone = true;
					_zoneTimer = ZoneStayTime + Random.Range(-ZoneStayTimeVariance , ZoneStayTimeVariance);
					
					_myNavMeshAgent.speed = WalkSpeed;
				}
				else if(Vector3.Distance(transform.position, _currentTargetPositionInZone) < 1f)
				{
					_myAnimator.SetFloat("Turn", 0f);

					_myNavMeshAgent.speed = 0f;
					
					_zoneTimer -= Time.deltaTime;
					
					if(_zoneTimer <= 0f)
					{
						_hasTargetInZone = false;
					}
					
				}	

			}


			
			if(myCurrentBehavior == CurrentBehavior.PathBased)
			{			
				_myNavMeshAgent.speed = WalkSpeed;

				_myNavMeshAgent.SetDestination(currentPoint.Current.transform.position);

				if(currentPoint == null || currentPoint.Current == null)
					return;
				var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
				if(distanceSquared < GoalDistance * GoalDistance)
					currentPoint.MoveNext();

				
				//Compare angle we were at to angle we're changing to and smooth it and devide by 4 to get a value between 0 and 1 for how much we turn.
				float angle = Vector3.Angle(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
				Vector3 cross = Vector3.Cross(_myNavMeshAgent.velocity, _myNavMeshAgent.desiredVelocity);
				if(cross.y < 0) angle = -angle;
				
				
				angle = Mathf.SmoothDamp(_lastAngle, angle, ref _refAngle, .5f);
				
				//Debug.Log(Vector3.Magnitude(_myNavMeshAgent.velocity).ToString());
				_myAnimator.SetFloat("Forward", _myNavMeshAgent.speed/(WalkSpeed*1.5f));
				_myAnimator.SetFloat("Turn", angle/4f);
			}
	}

		//Look at the player

		//Are we facing the player
		Vector3 neededRotationToPlayer;
		Quaternion hipsRotation;

		hipsRotation = MyHips.rotation;

		MyHips.LookAt(_player.transform);
		neededRotationToPlayer = MyHips.transform.rotation.eulerAngles;

		MyHips.rotation = hipsRotation;
	

		float hitAngle = Mathf.DeltaAngle(neededRotationToPlayer.y, MyHips.transform.eulerAngles.y);

		float headTurnCurrent = _myAnimator.GetFloat("HeadTurn");
		float HeadTurnAmount;

		if( hitAngle > -90F &&  hitAngle < 80F)
		{
			HeadTurnAmount = (( (hitAngle + 90f) / (80f + 90f) ) * (1f + 1f) - 1f) * -1f;
		}
		else
		{
			HeadTurnAmount = 0f;
		}
		_myAnimator.SetFloat("HeadTurn",Mathf.SmoothDamp(headTurnCurrent, HeadTurnAmount, ref _refHeadTurn, .4f));  


		if(_talkingDistance)
		{

			_myAnimator.SetFloat("Forward", Mathf.SmoothDamp(_myAnimator.GetFloat("Forward"), 0f, ref _refSpeed, .2f));  
			_myAnimator.SetFloat("Turn", Mathf.SmoothDamp(_myAnimator.GetFloat("Turn"), 0f, ref _refAngle, .2f));  

		}

	}

	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "Player") 
		{
			_myAnimator.SetBool("Talking", true);
		}
	}
	void OnTriggerExit(Collider col) 
	{
		if(col.gameObject.name == "Player") {
			_myAnimator.SetBool("Talking", false);
			_talkingDistance = false;
		}
	}

	void OnTriggerStay(Collider col) {
		if(col.gameObject.name == "Player") 
		{
			_myNavMeshAgent.speed = 0f;
			_talkingDistance = true;
		}
	}

}
