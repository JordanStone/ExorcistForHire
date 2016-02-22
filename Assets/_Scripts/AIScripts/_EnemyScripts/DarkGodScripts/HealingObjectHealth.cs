using UnityEngine;
using System.Collections;
using BossDetectionScript;

public class HealingObjectHealth : MonoBehaviour {

	public float healingObjectStartingHealth = 20f;
	[SerializeField]
	private float healingObjectHealth;
	public float BulletDamage = 30f;
	private DarkGodStateMachine _bossDetectionScript;

	void Start()
	{
		healingObjectHealth = healingObjectStartingHealth;
		_bossDetectionScript = GetComponentInParent<DarkGodStateMachine>();

	}

	void OnDestroy()
	{
		_bossDetectionScript.OnHealingObjectDeath();
	}

	void OnCollisionEnter(Collision collision)
	{ 
		
		if (collision.gameObject.tag == "Bullet")
		{
			healingObjectHealth -= BulletDamage;

			if(healingObjectHealth <= 0f)
			{
				Destroy(gameObject);
			}
		}
	}
}
