using UnityEngine;
using System.Collections;
using NashTools;
using EnemyDetectionScript;
using SoundController;


public class CultistAttackAndHealth : MonoBehaviour {

	public float DamageDeltToPlayer = 30f;
	public float GhostStartingHealth = 100f;
	private float _ghostHealth;
	public float BulletDamage = 30f;

	private EnemyStateMachine _enemyDetectionScript;

	private AudioSource spookyAttack;
	private AudioSource spookDeath;

	private SoundManager _soundManager;
	private Animator _myAnimator;


	// Use this for initialization
	void OnEnable () 
	{
		_myAnimator = GetComponentInParent<Animator>();

		_enemyDetectionScript = gameObject.GetComponentInChildren<EnemyStateMachine>();
		_ghostHealth = GhostStartingHealth;

//		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();

//		spookyMoan = GetComponents<AudioSource>()[0];
		spookyAttack = GetComponents<AudioSource>()[1];
		spookDeath = GetComponents<AudioSource>()[2];
	}
	
	// Update is called once per frame
	void Update () 
	{


	}

	
	void OnCollisionEnter(Collision collision)
	{
	
		if( collision.gameObject.name == "Player")
		{
			if(_enemyDetectionScript.currentState == EnemyStateMachine.EnemyState.Attack)
			{
				EventManager.PostNotification((int) GameManagerScript.GameEvents.DamagePlayer, this, DamageDeltToPlayer);
			}
		}

		if( collision.gameObject.tag == "Bullet")
		{
			_ghostHealth -= BulletDamage;
			_myAnimator.SetTrigger("HitStomache");

			if(_ghostHealth <= 0f)
			{
				if (spookDeath.clip)
					AudioSource.PlayClipAtPoint(spookDeath.clip, transform.position);
				_myAnimator.SetTrigger("DeathKnockdown");

				StartCoroutine(_enemyDetectionScript.ChangeState(EnemyStateMachine.EnemyState.Die));
			}
		}
	}

	public void HeadDamage()
	{
		if(_ghostHealth > 0f)
		{
			_ghostHealth -= BulletDamage*1.5f;
			_myAnimator.SetTrigger("HitFace");
		}
	}


}