using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnemyDetectionScript;

public class TriggerXEnemiesOnPlayerZoneEnter : MonoBehaviour {

	public GameObject[] Enemies;

	private List<EnemyStateMachine> _stateMachines;

	// Use this for initialization
	void Start () 
	{ 
		List<EnemyStateMachine> stateMachines = new List<EnemyStateMachine>();
		foreach (GameObject Enemy in Enemies)
		{	
			stateMachines.Add(Enemy.GetComponent<EnemyStateMachine>());

		}

		_stateMachines = stateMachines;
	
	}

	void OnTriggerEnter(Collider other) 
	{
		if(other.gameObject.tag == "Player")
		{
			foreach (EnemyStateMachine StateMachine in _stateMachines)
			{
				StateMachine.SetPlayerInTrigger(true);
			}
		}
	}


}
