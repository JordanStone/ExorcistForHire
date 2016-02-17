using System.Collections.Generic;
using UnityEngine;
using EnemyDetectionScript;

public class FollowPath : MonoBehaviour {


	private NavMeshAgent _myNavMeshAgent;

	public PathDefinition Path;
	private EnemyStateMachine _enemyDetectionScript;

	public float GoalDistance = .1f;
	private IEnumerator<Transform> currentPoint;
	private EnemyStateMachine detectState;
	public Transform AttackTarget;
	private Vector3 _lastTransform;


	public void Start() 
	{
		_myNavMeshAgent = GetComponent<NavMeshAgent>();

		AttackTarget = GameObject.Find("Player").transform;

		_enemyDetectionScript = gameObject.GetComponentInChildren<EnemyStateMachine>();

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
	}
	public void Update()
	{

		if(_enemyDetectionScript.currentState == EnemyStateMachine.EnemyState.Attack)
		{
			_myNavMeshAgent.SetDestination(AttackTarget.position);
		}

		if(_enemyDetectionScript.currentState == EnemyStateMachine.EnemyState.Patrol && _enemyDetectionScript.MovementType != EnemyDetectionScript.EnemyStateMachine.EnemyMovementType.ZoneBased)
		{
			_myNavMeshAgent.SetDestination(currentPoint.Current.transform.position);
		}	
		if(_enemyDetectionScript.currentState ==EnemyStateMachine.EnemyState.Die || _enemyDetectionScript.currentState == EnemyStateMachine.EnemyState.Search)
		{

		}


	}

	public void Patrol() {
		if(currentPoint == null || currentPoint.Current == null)
			return;
		var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
		if(distanceSquared < GoalDistance * GoalDistance)
			currentPoint.MoveNext();

	}





}
