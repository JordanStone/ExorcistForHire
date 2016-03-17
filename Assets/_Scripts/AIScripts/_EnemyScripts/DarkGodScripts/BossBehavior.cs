﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BossDetectionScript;
using SoundController;
using NashTools;

public class BossBehavior : MonoBehaviour {

	public enum BossAttack{ Claw, Petrification, Aoe, Ranged};
	public float bossStartingHealth = 100f;
	[SerializeField]
	private float bossHealth;
	public float MeleeDamageDeltToPlayer = 30f;
	public float bulletDamage = 30f;
	public float healInterval = 0.3f;
	public float healAmount = 45f;
	public float clawBaseProbability = 50f;
	public float petrificationBaseProbability = 5f;
	public float aoeBaseProbability = 10f;
	public float rangedBaseProbability = 35f;
	public float aoeKnockback = 1000f;
	private BossAttack lastAttack;
	public GameObject petrificationObject;
	public GameObject rangedObject;
	private SphereCollider aoeCollider;
	private BossAttack currentAttack;
	private GameObject aoeSphere;
	private bool changeState = false;
	public bool inAttack = false;
	private bool strawDeployed = false;

	public class AttackData
	{
		public BossAttack attack;
		public float probability;
		public float damage;
		public int successfulHits;
		public int totalTimesUsed;

		public AttackData(){}
		public void changeProbability(float value)
		{
			probability += value;
		}

		public void incrementTimesUsed()
		{
			totalTimesUsed++;
		}

	}
	private List<AttackData> availableAttacks;
	private AttackData clawData = new AttackData();
	private AttackData petrificationData = new AttackData(); 
	private AttackData aoeData = new AttackData();
	private AttackData rangedData = new AttackData();
	private DarkGodStateMachine _bossDetectionScript;
	private Animator _myAnimator;

	private AudioSource spookyAttack;
	private AudioSource spookDeath;

	private SoundManager _soundManager;

	private bool healing = false;
	private bool startHealing = false;
	private GameObject healingObject;
	private Transform petrifySpawn;
	private Transform projectileSpawn;

	// Use this for initialization
	void Start () {
		availableAttacks = new List<AttackData>();
		InitializeAttackData();
		availableAttacks.Add(clawData);
		_bossDetectionScript = gameObject.GetComponentInChildren<DarkGodStateMachine>();
		_myAnimator = gameObject.GetComponent<Animator>();
		healingObject = transform.Find("HealingObject").gameObject;

		bossHealth = bossStartingHealth;

		//spookyAttack = GetComponents<AudioSource>()[1];
		//spookDeath = GetComponents<AudioSource>()[2];
		healingObject.SetActive(false);
		aoeCollider = GetComponent<SphereCollider>();
		aoeCollider.enabled = false;
		aoeSphere = transform.Find("AoeSphere").gameObject;
		aoeSphere.SetActive(false);
		petrifySpawn = transform.Find("PetrifyLocation").transform;
		projectileSpawn = transform.Find("ProjectileSpawn").transform;
	}
	
	// Update is called once per frame
	void Update () {

		if(startHealing && !healing && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Two)
		{
			StartCoroutine(Heal());
		}
		if(changeState == true)
		{
			_bossDetectionScript.Speed = 0f;
		}
		/*if(Input.GetKeyDown(KeyCode.Tab))
		{
			AttackData testAttack = ChooseAttack();
			Debug.Log("Reported Attack: " + testAttack.attack);
		}*/
	
	}

	private AttackData ChooseAttack()
	{
		SortAttacksByProbability();
		float randomBase = Random.value * 100f; //Gives a random number between 0 and 100

		for(int i = 0; i < availableAttacks.Count; i++)
		{
			if (randomBase < availableAttacks[i].probability)
			{
				//Debug.Log("Chose " + availableAttacks[i].attack + " Attack with probability: " + availableAttacks[i].probability);
				return availableAttacks[i];
			}
			else
			{
				randomBase -= availableAttacks[i].probability;
			}

		}
		//If no attack is determined, choose one with largest probability
		return availableAttacks[0];

	}

	public void AttackPlayer (float distanceToTarget)
	{
		SetProbabilitiesOfAttacks(distanceToTarget);
		AttackData chosenAttack = ChooseAttack();

		switch(chosenAttack.attack)
		{
		case BossAttack.Claw:
			ClawAttack();
			break;
		case BossAttack.Petrification:
			PetrificationAttack();
			break;
		case BossAttack.Aoe:
			AoeAttack();
			break;
		case BossAttack.Ranged:
			RangedAttack();
			break;
		}
	}
	public void CheckListOrder()
	{
		int i = 0;
		Debug.Log("Before Sort: ");
		foreach (AttackData ad in availableAttacks)
		{
			Debug.Log("#" + i + " Attack: " + ad.attack + " Probability: " + ad.probability);
			i++;
		}
		SortAttacksByProbability();
		int j=0;
		Debug.Log("After Sort: ");
		foreach (AttackData ad in availableAttacks)
		{
			Debug.Log("#" + j + " Attack: " + ad.attack + " Probability: " + ad.probability);
			j++;
		}
	}

	private void InitializeAttackData()
	{
		//Initialize Claw attack data
		clawData.attack = BossAttack.Claw;
		clawData.probability = clawBaseProbability;
		clawData.damage = 30f;
		clawData.successfulHits = 0;
		clawData.totalTimesUsed = 0;

		//Initialize Petrification attack data
		petrificationData.attack = BossAttack.Petrification;
		petrificationData.probability = petrificationBaseProbability;
		petrificationData.damage = 0f;
		petrificationData.successfulHits = 0;
		petrificationData.totalTimesUsed = 0;

		//Initialize AOE attack data
		aoeData.attack = BossAttack.Aoe;
		aoeData.probability = aoeBaseProbability;
		aoeData.damage = 40f;
		aoeData.successfulHits = 0;
		aoeData.totalTimesUsed = 0;

		//Initialize ranged attack data
		rangedData.attack = BossAttack.Ranged;
		rangedData.probability = rangedBaseProbability;
		rangedData.damage = 0f;
		rangedData.successfulHits = 0;
		rangedData.totalTimesUsed = 0;
	}

	private void SortAttacksByProbability()
	{
		//Sort list by probability property of attack data struct. Greatest to smallest.
		availableAttacks.Sort((b, a) => a.probability.CompareTo(b.probability));
		/*for(int i=0; i<availableAttacks.Count; i++)
		{
			Debug.Log("Attack Type: " + availableAttacks[i].attack + " Probability: " + availableAttacks[i].probability);
		}*/

	}

	public void AddPhaseAttacks(DarkGodStateMachine.BossPhase phase)
	{
		switch(phase)
		{
		case DarkGodStateMachine.BossPhase.Two:
			Debug.Log("Adding phase 2 Attacks");
			availableAttacks.Add(rangedData);
			break;
		case DarkGodStateMachine.BossPhase.Three:
			Debug.Log("Adding phase 3 attacks");
			availableAttacks.Add(petrificationData);
			availableAttacks.Add(aoeData);
			break;
		}

	}

	private void ClawAttack()
	{
		if(_bossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
		{
			currentAttack = BossAttack.Claw;
			_bossDetectionScript.Speed = 0f;
			//spookyAttack.Play();
			_myAnimator.SetBool("IsWalking", false);
			_myAnimator.SetTrigger("ClawAttack");
			//clawData.totalTimesUsed += 1;
			lastAttack = BossAttack.Claw;
			StartCoroutine(WaitForAttackToFinish(0.7f, false, "none", 0.5f));

			//Debug.Log ("CLAAAAAAAAAAAAAWWWW!!!");
		}
	}

		private void PetrificationAttack()
	{
		if(_bossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
		{
			currentAttack = BossAttack.Petrification;
			_bossDetectionScript.Speed = 0f;
			_myAnimator.SetBool("IsWalking", false);
			_myAnimator.SetTrigger("PetrifyAttack");
			//Debug.Log ("PETRRRRRIFFFFY!!!");
			StartCoroutine(WaitForAttackToFinish(2.5f, true, "petrify", 0.7f));
			//GameObject petrify = Instantiate(petrificationObject, transform.position + new Vector3(0f, 1f, 1f), Quaternion.identity)as GameObject;
			//petrify.transform.SetParent(this.transform);

			//petrificationData.totalTimesUsed += 1;
			lastAttack = BossAttack.Petrification;
		}
	}
		private void AoeAttack()
	{
		if(_bossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
		{
			currentAttack = BossAttack.Aoe;
			_bossDetectionScript.Speed = 0f;
			_myAnimator.SetBool("IsWalking", false);
			_myAnimator.SetTrigger("TentacleAttack");
			//Debug.Log ("AAAAAAOOOOOOOOEEEEE!!!");
			StartCoroutine(WaitForAttackToFinish(2f, false, "aoe", 0.8f));

			//aoeData.totalTimesUsed += 1;
			//StartCoroutine(ActivateAOE());

			lastAttack = BossAttack.Aoe;
		}
	}
		private void RangedAttack()
	{
		if(_bossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
		{
			currentAttack = BossAttack.Ranged;
			_bossDetectionScript.Speed = 0f;
			_myAnimator.SetBool("IsWalking", false);
			_myAnimator.SetTrigger("RangedAttack");
			StartCoroutine(WaitForAttackToFinish(1.2f, true, "range", 0.7f));
			//GameObject range = Instantiate(rangedObject, projectileSpawn.transform.position, Quaternion.identity)as GameObject;
			//StartCoroutine( _bossDetectionScript.ChangeState(DarkGodStateMachine.BossState.Search));
			//Debug.Log ("RAAAAAAAAAAAAAAANGE!!!");

			//rangedData.totalTimesUsed += 1;
			lastAttack = BossAttack.Ranged;
		}
	}

	private IEnumerator ActivateAOE()
	{
		aoeCollider.enabled = true;
		aoeSphere.SetActive(true);
		float aoeIncrement = 0.3f;
		while(aoeCollider.radius < 2.9f)
		{
			aoeCollider.radius += aoeIncrement;
			aoeSphere.transform.localScale += new Vector3(aoeIncrement, aoeIncrement, aoeIncrement);
			yield return new WaitForSeconds(0.01f);
		}

		yield return new WaitForSeconds(0.7f);

		aoeSphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		aoeSphere.SetActive(false);
		aoeCollider.radius = 0.5f;
		aoeCollider.enabled = false;
	}

	private void SetProbabilitiesOfAttacks(float distanceToTarget)
	{
		for(int i=0; i<availableAttacks.Count; i++)
		{
			if(availableAttacks[i].attack == BossAttack.Claw)
				SetProbabilityOfClaw(i, distanceToTarget);
			else if(availableAttacks[i].attack == BossAttack.Petrification)
				SetProbabilityOfPetrification(i, distanceToTarget);
			else if(availableAttacks[i].attack == BossAttack.Aoe)
				SetProbabilityOfAoe(i, distanceToTarget);
			else if(availableAttacks[i].attack == BossAttack.Ranged)
				SetProbabilityOfRanged(i, distanceToTarget);

			//Debug.Log("Probability of " + i + ": " + availableAttacks[i].probability);
		}
	}

	private void SetProbabilityOfClaw(int index, float distanceToTarget)
	{
		float lostPoints = 0f;
		bool rangedOnly = false;

		if(_bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.One)
		{
			return;
		}
		if(distanceToTarget >= 5f)
		{
			lostPoints += availableAttacks[index].probability;
			//availableAttacks[index].probability = 0f;
			//Debug.Log("Claw -> Ranged");

			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability);
			//Debug.Log("Claw probability: " + availableAttacks[index].probability);

			rangedOnly = true;
		}
		if(lastAttack == clawData.attack && availableAttacks[index].probability != 0f && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
		{
			lostPoints += availableAttacks[index].probability * 0.25f;
			//availableAttacks[index].probability -= availableAttacks[index].probability * 0.25f;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability * 0.25f);
		}

		for(int i=0; i<availableAttacks.Count; i++)
		{
			if(rangedOnly && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
			{
				if(availableAttacks[i].attack == BossAttack.Ranged)
				{
					//availableAttacks[i].probability += lostPoints * 0.75f;
					availableAttacks[i].changeProbability(lostPoints * 0.75f);
				}
				else if(availableAttacks[i].attack == BossAttack.Petrification)
				{
					//availableAttacks[i].probability += lostPoints * 0.25f;
					availableAttacks[i].changeProbability(lostPoints * 0.25f);
				}

			}
			else if (rangedOnly)
			{
				if(availableAttacks[i].attack == BossAttack.Ranged)
				{
					//availableAttacks[i].probability += lostPoints * 0.75f;
					availableAttacks[i].changeProbability(lostPoints);
				}
			}
			else
			{
				if(availableAttacks[i].attack == BossAttack.Aoe && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
				{
					//availableAttacks[i].probability += lostPoints;
					availableAttacks[i].changeProbability(lostPoints);
				}
			}

		}
		//Debug.Log("Claw probability: " + availableAttacks[index].probability);
	}

	private void SetProbabilityOfPetrification(int index, float distanceToTarget)
	{
		float lostPoints = 0f;
		bool rangedOnly = true;
		if(distanceToTarget < 5f)
		{
			lostPoints += availableAttacks[index].probability;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability);
			rangedOnly = false;
		}
		if(lastAttack == petrificationData.attack && availableAttacks[index].probability != 0f)
		{
			lostPoints += availableAttacks[index].probability * 0.9f;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability * 0.9f);
		}

		for(int i=0; i<availableAttacks.Count; i++)
		{
			if(!rangedOnly)
			{
				if(availableAttacks[i].attack == BossAttack.Claw)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.75f);
				}
				else if(availableAttacks[i].attack == BossAttack.Aoe)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.25f);
				}

			}
			else
			{
				if(availableAttacks[i].attack == BossAttack.Ranged)
				{
					availableAttacks[i].changeProbability(lostPoints);
				}
			}

		}
	}

	private void SetProbabilityOfAoe(int index, float distanceToTarget)
	{
		float lostPoints = 0f;
		bool rangedOnly = false;
		if(distanceToTarget >= 5f)
		{
			lostPoints += availableAttacks[index].probability;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability);
			rangedOnly = true;
		}
		if(lastAttack == aoeData.attack && availableAttacks[index].probability != 0f)
		{
			lostPoints += availableAttacks[index].probability * 0.8f;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability * 0.8f);
		}

		for(int i=0; i<availableAttacks.Count; i++)
		{
			if(rangedOnly)
			{
				if(availableAttacks[i].attack == BossAttack.Ranged)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.75f);
				}
				else if(availableAttacks[i].attack == BossAttack.Petrification)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.25f);
				}

			}
			else
			{
				if(availableAttacks[i].attack == BossAttack.Claw)
				{
					availableAttacks[i].changeProbability(lostPoints);
				}
			}

		}

	}

	private void SetProbabilityOfRanged(int index, float distanceToTarget)
	{
		float lostPoints = 0f;
		bool rangedOnly = true;
		if(distanceToTarget < 5f)
		{
			lostPoints += availableAttacks[index].probability;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability);
			rangedOnly = false;
		}
		if(lastAttack == rangedData.attack && availableAttacks[index].probability != 0f && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
		{
			lostPoints += availableAttacks[index].probability * 0.33f;
			availableAttacks[index].changeProbability(-1f * availableAttacks[index].probability * 0.33f);
		}

		for(int i=0; i<availableAttacks.Count; i++)
		{
			if(!rangedOnly && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
			{
				if(availableAttacks[i].attack == BossAttack.Claw)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.75f);
				}
				else if(availableAttacks[i].attack == BossAttack.Aoe)
				{
					availableAttacks[i].changeProbability(lostPoints * 0.25f);
				}

			}
			else if (!rangedOnly)
			{
				if(availableAttacks[i].attack == BossAttack.Claw)
				{
					availableAttacks[i].changeProbability(lostPoints);
				}
			}
			else
			{
				if(availableAttacks[i].attack == BossAttack.Petrification && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Three)
				{
					availableAttacks[i].changeProbability(lostPoints);
				}
			}

		}
	}

	public float GetBossHealth()
	{
		return bossHealth;
	}

	public void HealHealth()
	{
		if(healingObject != null)
		{
			_myAnimator.SetBool("IsWalking", false);
			_myAnimator.SetBool("IsHealing", true);
			if(!strawDeployed)
				StartCoroutine(DeployStraw(2f, 0.8f));
		}

	}

	IEnumerator DeployStraw(float time, float split)
	{
		float remainder = 1f - split;
		yield return new WaitForSeconds(time * split);
		if(healingObject != null)
		{
			healingObject.SetActive(true);
			strawDeployed = true;
		}
		startHealing = true;
		yield return new WaitForSeconds(time * remainder);


	}

	public void StopHealing()
	{

		if(healingObject != null)
		{
			healingObject.SetActive(false);
			strawDeployed = false;
		}

		_myAnimator.SetBool("IsWalking", true);
		_myAnimator.SetBool("IsHealing", false);
		startHealing = false;
	}

	IEnumerator Heal()
	{
		healing = true;
		yield return  new WaitForSeconds(healInterval);
		bossHealth += healAmount;
		healing = false;
		if(bossHealth >= bossStartingHealth)
		{
			bossHealth = bossStartingHealth;
			StartCoroutine(_bossDetectionScript.ChangeState(DarkGodStateMachine.BossState.Search));
		}

	}

	IEnumerator WaitForAttackToFinish(float time, bool ranged, string name, float split)
	{
		changeState = false;
		inAttack = true;
		float remainder = 1f - split;
		if (ranged)
		{
			yield return new WaitForSeconds(time * split);
			if (name == "range")
			{
				GameObject range = Instantiate(rangedObject, projectileSpawn.transform.position, Quaternion.identity)as GameObject;
			}
			else
			{
				GameObject petrify = Instantiate(petrificationObject, petrifySpawn.transform.position, Quaternion.identity)as GameObject;
				petrify.transform.SetParent(this.transform);
			}

			yield return new WaitForSeconds(time * remainder);
		}
		else
		{
			if (name == "aoe")
			{
				yield return new WaitForSeconds(time * split);
				StartCoroutine(ActivateAOE());
				yield return new WaitForSeconds(time * remainder);
			}
			else
			{
				yield return new WaitForSeconds(time);
			}

		}
		inAttack = false;
		changeState = true;
		StartCoroutine( _bossDetectionScript.ChangeState(DarkGodStateMachine.BossState.Search));
	}

	void OnCollisionEnter(Collision collision)
	{

		if( collision.gameObject.name == "Player")
		{
			if(_bossDetectionScript.currentState == DarkGodStateMachine.BossState.Attack)
			{
				EventManager.PostNotification((int) GameManagerScript.GameEvents.DamagePlayer, this, MeleeDamageDeltToPlayer);
				if(currentAttack == BossAttack.Aoe)
				{
					Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
					rb.AddForce(collision.gameObject.transform.forward * (-1f) * aoeKnockback);
				}
			}
		}
		if (collision.gameObject.tag == "Bullet" && _bossDetectionScript.currentPhase != DarkGodStateMachine.BossPhase.One)
		{
			bossHealth -= bulletDamage;

			if(bossHealth < (bossStartingHealth/2f) && _bossDetectionScript.currentPhase == DarkGodStateMachine.BossPhase.Two)
			{
				//Debug.Log("HEALTH at need healing state");
				StartCoroutine(_bossDetectionScript.ChangeState(DarkGodStateMachine.BossState.Heal));
			}
			if(bossHealth <= 0f)
			{
				_bossDetectionScript.Speed = 0f;	
				_myAnimator.SetBool("IsWalking", false);
				_myAnimator.SetBool("IsHealing", false);
				_myAnimator.SetBool("Dying", true);

				StartCoroutine(_bossDetectionScript.ChangeState(DarkGodStateMachine.BossState.Die));
			}
		}
	}

}
