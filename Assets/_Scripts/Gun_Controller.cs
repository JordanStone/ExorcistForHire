using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoundController;

namespace GunController
{
	public class Gun_Controller : MonoBehaviour {
		
		//Public Gameobjects
		public Animator gunBodyAnimator;
		public Animator gunBarrelAnimator;
		public float fireTime;
		public float reloadTime;
		public GameObject[] bulletSpawners;
		public GameObject[] bullets;
		public GameObject bullet;
		
		
		//For the bullet that actually flies
		public GameObject FiredBulletSpawnLocation;
		public GameObject FiredBullet;
		
		//Keeps track of which bullet in the array to delete, as well as how many bullets to spawn.
		private int _bulletsArrayNav;
		
		
		public bool _firing;
		public bool _reloading;
		public static int _ammoCount = 6;
		public float _reloadTimer;
		public float _fireTimer;
		
		private bool _Equipped = false;
		
		public AudioSource gunShot;	
		private SoundManager _soundManager;
		
		// Use this for initialization
		void Start() 
		{
			
			_ammoCount = 6;
			_firing = false;
			_reloading = false;
			_reloadTimer = 1;
			gunBarrelAnimator.enabled = false;
			//We start by spawning 6 bullets in.
			_bulletsArrayNav = 6;
			SpawnBullets();
			//Then we only want 0 to spawn.
			_bulletsArrayNav = 0;
			
			_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
			gunShot.clip = _soundManager.GetGunSound(0);
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (_Equipped == true && GameManagerScript.isPaused == false) 
			{
				if (Input.GetButtonDown ("Fire1") && _firing == false && _ammoCount >= 1 && _reloading == false && _fireTimer <= 0f) {
					FireBullet ();
					gunBarrelAnimator.enabled = true;
					_firing = true;
					_ammoCount -= 1;
					gunBodyAnimator.SetTrigger ("Fire");
					gunBarrelAnimator.SetTrigger ("Fire");
					_fireTimer = fireTime;
					bullets [_bulletsArrayNav].SendMessage ("DestroyMe");
					_bulletsArrayNav += 1;
				}
				
				_fireTimer -= Time.deltaTime;
				
				if (Input.GetKeyDown (KeyCode.R) && _ammoCount != 6 && _reloading == false && _firing == false) {
					gunBarrelAnimator.enabled = true;
					gunBodyAnimator.SetTrigger ("Reload");
					gunBarrelAnimator.SetTrigger ("Reload");
					_reloading = true;
					_reloadTimer = reloadTime;
				}
				if (Input.GetButtonDown ("Fire1") && _ammoCount == 0f && _reloading == false && _firing == false) {
					gunBarrelAnimator.enabled = true;
					gunBodyAnimator.SetTrigger ("Reload");
					gunBarrelAnimator.SetTrigger ("Reload");
					_reloading = true;
					_reloadTimer = reloadTime;
				}
				
				//Decrease timer when gun is down.
				if (_reloading == true)
				{
					_reloadTimer -= Time.deltaTime;
				}
				
				//If reloadTime is 0 or less, tell the animator we're done reloading then set reload _timer to 1
				if (_reloadTimer <= 0 && _reloading == true)
				{
					//DeleteBullets();
					SpawnBullets();
					_reloadTimer = 1;
					gunBodyAnimator.SetTrigger("Done_Reloading");
					gunBarrelAnimator.SetTrigger("Done_Reloading");
					_ammoCount = 6;
					_bulletsArrayNav = 0;
				}
			}
		}
		
		
		//public GameObject[] bullets;
		void DeleteBullets()
		{
			for(int i = 0; i < bulletSpawners.Length; i++)
			{
				bullets[i].SendMessage("DestroyMe");
			}
		}
		
		void SpawnBullets()
		{	
			for(int i = 0; i < _bulletsArrayNav; i++)
			{
				GameObject aBullet = Instantiate(bullet, bulletSpawners[i].transform.position, bulletSpawners[i].transform.localRotation) as GameObject;
				aBullet.transform.parent = bulletSpawners[i].transform;
				bullets[i] = aBullet;
			}
		}
		
		void FireBullet()
		{
			gunShot.Play();
			Instantiate(FiredBullet, FiredBulletSpawnLocation.transform.position, FiredBulletSpawnLocation.transform.rotation);
		}
		
		
		void DoneFiring()
		{
			_firing = false;
		}
		
		void DoneReloading()
		{
			_reloading = false;
		}
		
		void PauseFire()
		{
			gunBarrelAnimator.enabled = false;
		}
		
		
		public void setEquipped(bool isEquipped)
		{
			_Equipped = isEquipped;
			
			if (_Equipped == false) 
			{
				gunBodyAnimator.SetTrigger("Reload");
				gunBodyAnimator.SetBool("Equipped", false);
			}
			else if (_Equipped == true) 
			{
				gunBodyAnimator.SetTrigger("Done_Reloading");
				gunBodyAnimator.SetBool("Equipped", true);
				
			}
			
		}
		
		
		public static int GetBulletCount()
		{
			return _ammoCount;
		}
		
	}
}