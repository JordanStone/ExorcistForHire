using UnityEngine;
using System.Collections;
using NashTools;
using DG.Tweening;

public class FinalBoss : MonoBehaviour 
{

	public float GhostStartingHealth = 100f;
	private float _ghostHealth;
	public float BulletDamage = 30f;

	public float DeathTime;

	private bool hasDied;

	private Animator _myAnimator;

	// Use this for initialization
	void Start () 
	{
		_myAnimator = GetComponent<Animator> ();

		EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);
	}
	
	// Update is called once per frame
	void Update () 
	{
			
	}

	void OnReset(Component poster, object checkpointData)
	{
		_ghostHealth = GhostStartingHealth;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Bullet") {
			_ghostHealth -= BulletDamage;
			
			if (_ghostHealth <= 0f) 
			{	
				StartCoroutine("Die");
			}
			
		}
	}

	IEnumerator Die()
	{

		_myAnimator.SetBool ("Dead", true);
	
		yield return new WaitForSeconds(DeathTime);


		if (!hasDied)
		{
			GameManagerScript.LockSwitch((int) GameManagerScript.GameEvents.Victory);
			print("there should be a victory screen now");
			hasDied = true;
		}

		Destroy(this);
	}
}
