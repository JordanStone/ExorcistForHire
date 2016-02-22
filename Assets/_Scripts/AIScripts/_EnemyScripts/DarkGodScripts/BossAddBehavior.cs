using UnityEngine;
using System.Collections;
using BossDetectionScript;

public class BossAddBehavior : MonoBehaviour {
	private DarkGodStateMachine _bossDetectionScript;

	// Use this for initialization
	void Start () {
		_bossDetectionScript = GameObject.Find("DarkGod").GetComponent<DarkGodStateMachine>();
	}
	
	//Cultitsts aren't destroyed apparently
	/*void OnDestroy()
	{
		_bossDetectionScript.OnBossAddDeath();
	}*/

	public void AddDeath()
	{
		_bossDetectionScript.OnBossAddDeath();
	}
}
