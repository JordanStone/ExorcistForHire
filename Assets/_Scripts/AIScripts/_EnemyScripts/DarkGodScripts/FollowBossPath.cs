using UnityEngine;
using System.Collections;
using BossDetectionScript;
using System.Collections.Generic;

public class FollowBossPath : MonoBehaviour {

	private NavMeshAgent _myNavMeshAgent;

	public PathDefinition Path;
	private DarkGodStateMachine _BossDetectionScript;
	private BossBehavior _bossBehaviorScript;

	public float GoalDistance = .1f;
	private IEnumerator<Transform> currentPoint;
	public Transform AttackTarget;
	public float cooldown = 3f;
	private bool attackCool;
	private Vector3 _lastTransform;
	private float minDistanceToTarget;
	private Vector3 fountainPosition;
	private float maxDistanceToTarget;
	private float baseAttackChance;


	public void Start() 
	{
		_myNavMeshAgent = GetComponent<NavMeshAgent>();

		AttackTarget = GameObject.Find("Player").transform;
		fountainPosition = GameObject.Find("Fountain").transform.position;

		minDistanceToTarget = 2f;
		maxDistanceToTarget = 80f;
		baseAttackChance = 25f;
		attackCool = false;


		_BossDetectionScript = gameObject.GetComponent<DarkGodStateMachine>();
		_bossBehaviorScript = gameObject.GetComponent<BossBehavior>();

		if(Path == null) {
			Debug.LogError("Path cannot be null.", gameObject);
			return;
		}
		currentPoint = Path.GetPathEnumerator();
		currentPoint.MoveNext();
		if(currentPoint.Current == null)
			return;
		transform.position = currentPoint.Current.position;
		_lastTransform = transform.position;
	}
	public void Update()
	{

		if(_BossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
		{
			_myNavMeshAgent.SetDestination(AttackTarget.position);
		}

		if(_BossDetectionScript.currentState == DarkGodStateMachine.BossState.Patrol)
		{
			//Debug.Log("_myNavMeshAgent is null? " + (_myNavMeshAgent == null));
			//Debug.Log("CurrentPoint is null? " + (currentPoint == null));
			if(currentPoint.Current != null)
				_myNavMeshAgent.SetDestination(currentPoint.Current.transform.position);
			//_myNavMeshAgent.SetDestination(AttackTarget.position);
		}	

		if (_BossDetectionScript.currentState == DarkGodStateMachine.BossState.Heal)
		{
			_myNavMeshAgent.SetDestination(fountainPosition);
		}
		if(_BossDetectionScript.currentState == DarkGodStateMachine.BossState.Die || _BossDetectionScript.currentState == DarkGodStateMachine.BossState.Search)
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
	public void Attack() 
	{
		float targetDistance = Vector3.Distance(AttackTarget.position, transform.position);
		if(targetDistance <= minDistanceToTarget)
		{
			_bossBehaviorScript.AttackPlayer(targetDistance);	
			//StartCoroutine(cooloff());
		}

		else if ((targetDistance > minDistanceToTarget) && (_BossDetectionScript.currentPhase != DarkGodStateMachine.BossPhase.One) && (attackCool == false))
		{
				float randomChance = Random.value * 100f;
				float chanceToAttack = ChanceOfAttack(targetDistance);
				if(randomChance < chanceToAttack)
				{
					_bossBehaviorScript.AttackPlayer(targetDistance);
				}
				StartCoroutine(cooloff());
		}

	}

	//Chance that boss will attack player based on phase and proximity to player
	//Should not be called in phase one
	private float ChanceOfAttack(float targetDistance)
	{
		float phaseAttack = (float)(baseAttackChance * (int)_BossDetectionScript.currentPhase/2);
		float tempDistance = targetDistance;
		if(tempDistance > maxDistanceToTarget)
		{
			tempDistance = maxDistanceToTarget;
		}
		float percentageDistance = (maxDistanceToTarget-targetDistance)/maxDistanceToTarget;
		if(percentageDistance < 0.25f)
		{
			percentageDistance = 0.25f;
		}
		float aggressiveness = 1f/(percentageDistance * .5f);
		float chance = phaseAttack + aggressiveness;
		return chance;

	}

	private IEnumerator cooloff()
	{
		//Debug.Log("Cooling");
		float cool = cooldown;
		if(_BossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
			cool = cooldown * 1.5f;
		attackCool = true;
		yield return new WaitForSeconds(cooldown);
		attackCool = false;
	}



}
