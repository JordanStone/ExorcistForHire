using UnityEngine;
using System.Collections;
using NashTools;
using EnemyDetectionScript;
using SoundController;


public class FinalBossGhostAttack : MonoBehaviour {
	
	public float DamageDeltToPlayer = 30f;

	private AudioSource spookyAttack;
	private AudioSource spookDeath;
	
	private SoundManager _soundManager;
	private Animator _myAnimator;
	
	
	// Use this for initialization
	void Start () 
	{
		_myAnimator = GetComponentInParent<Animator>();
	
		//_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();

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
			spookyAttack.Play();
			_myAnimator.SetTrigger("Attack");
			_myAnimator.SetBool("Chasing",false);
					
			EventManager.PostNotification((int) GameManagerScript.GameEvents.DamagePlayer, this, DamageDeltToPlayer);
		}
	}
}

