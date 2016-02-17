using UnityEngine;
using System.Collections;

public class EnemyHeadDamage : MonoBehaviour {

	public CultistAttackAndHealth MyGhostAttackAndHealth;

	void OnCollisionEnter(Collision collision)
	{
		if( collision.gameObject.tag == "Bullet")
		{	
			MyGhostAttackAndHealth.HeadDamage();
		}
	}

}
