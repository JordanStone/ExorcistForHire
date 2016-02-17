using UnityEngine;
using System.Collections;
using NashTools;
using EnemyDetectionScript;
using SoundController;


public class GhostAttackAndHealth : MonoBehaviour {

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
	void Start () 
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
				spookyAttack.Play();
				_myAnimator.SetTrigger("Attack");
				_myAnimator.SetBool("Chasing",false);
				StartCoroutine(	_enemyDetectionScript.ChangeState(EnemyStateMachine.EnemyState.Search));

				EventManager.PostNotification((int) GameManagerScript.GameEvents.DamagePlayer, this, DamageDeltToPlayer);
			}
		}

		if( collision.gameObject.tag == "Bullet")
		{
			_ghostHealth -= BulletDamage;

			if(_ghostHealth <= 0f)
			{
//				spookDeath.Play();
				if (spookDeath.clip)
					AudioSource.PlayClipAtPoint(spookDeath.clip, transform.position);
				_myAnimator.SetBool("Dead", true);

				StartCoroutine(_enemyDetectionScript.ChangeState(EnemyStateMachine.EnemyState.Die));

			}

		}

	}
}
