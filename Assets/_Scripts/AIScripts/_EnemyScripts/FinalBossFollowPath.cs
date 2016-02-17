using System.Collections.Generic;
using UnityEngine;
using EnemyDetectionScript;

public class FinalBossFollowPath : MonoBehaviour {
	public enum FollowType 
	{
		MoveTowards,
		Lerp
	}
	public FollowType Type = FollowType.MoveTowards;
	public PathDefinition Path;
	public float _speed = 10f;
	private EnemyStateMachine _enemyDetectionScript;
	
	
	public float GoalDistance = .1f;
	private IEnumerator<Transform> currentPoint;
	private EnemyStateMachine detectState;
	public Transform AttackTarget;
	private Vector3 _lastTransform;
	public float RotationFactor = 100f;
	
	public void Start() 
	{
		AttackTarget = GameObject.Find("Player").transform;

	}
	public void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, AttackTarget.position, Time.deltaTime * _speed);
		gameObject.transform.LookAt( (transform.position - _lastTransform) * RotationFactor);
		_lastTransform = transform.position;
	}
	

	
	
	
}
