using UnityEngine;
using System.Collections;
using NashTools;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;
using CheckpointClass;

public class PlayerStats : MonoBehaviour 
{
	private float _health;
	public float StartingHealth = 100f;

	private VignetteAndChromaticAberration CameraEffects;

	public float InvincibilityTime = 1f;
	private float _invincibilityTimer;
	private bool _invincible = false;

	public float maxChromaticAberration = 50f;
	public float ChromaticAbarrationTime = 2f;

	// Use this for initialization
	void Start () 
	{
		_health = StartingHealth;
		EventManager.AddListener((int) GameManagerScript.GameEvents.DamagePlayer, OnHit);
		CameraEffects = Camera.main.GetComponent<VignetteAndChromaticAberration>();
		EventManager.AddListener((int) GameManagerScript.GameEvents.LoadlastCheckpoint, OnLoadLastCheckpoint);
	}

	// Update is called once per frame
	void Update () 
	{
		if(_invincible == true)
		{
			_invincibilityTimer -= Time.deltaTime;

			if(_invincibilityTimer <= 0f)
			{
				_invincible = false;
			}

		}
		

	}

	void OnHit(Component poster, object DamageAmmount)
	{
		if(!_invincible)
		{
			_health -= (float) DamageAmmount;
			_invincible = true;
			_invincibilityTimer = InvincibilityTime;
			CameraEffects.chromaticAberration = maxChromaticAberration;
			DOTween.To(()=> CameraEffects.chromaticAberration, x=> CameraEffects.chromaticAberration = x, 0f, ChromaticAbarrationTime);
		}
		if (_health <= 0f)
		{
			GameManagerScript.LockSwitch((int) GameManagerScript.GameEvents.Dead);
//			EventManager.PostNotification((int) GameManagerScript.GameEvents.Dead, this, true);
		}
	}

	public float GetHealth()
	{
		return _health;
	}

	void OnLoadLastCheckpoint(Component poster, object checkpointData)
	{
		CheckpointClass.Checkpoint.CheckpointData newCheckpointData = (CheckpointClass.Checkpoint.CheckpointData) checkpointData;

		_health = newCheckpointData.Health;
		transform.position = newCheckpointData.SpawnLocation;
	}

	public void Gethit(float damage){
		_health-=damage;
	}
}

