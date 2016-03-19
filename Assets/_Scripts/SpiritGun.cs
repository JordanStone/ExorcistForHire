using UnityEngine;
using System.Collections;
using SoundController;
using UnityEngine.UI;
using NashTools;
using CheckpointClass;

namespace GunController
{
	public class SpiritGun : MonoBehaviour {

		//Six shooters have six bullets.
		private int _bullets = 6;

		//Reserve ammo is ammunition not in the gun
		//maxBullets represents the cylinder size
		public int reserveAmmo;
		private int maxBullets = 6;

		//Time each shot takes.
		public float FireRate;
		private float _fireRateTimer;

		//Time it takes to reload. This is between the two reload animations.
		public float ReloadTime;
		private float _reloadTimer;
		private bool _reloading;


		//The hit spark that appears on what was shot.
		public GameObject HitSpark;
		//The muzzle flash that happens when we shoot.
		public GameObject MuzzleFlash;
		//The location of the muzzle flash.
		public GameObject MuzzleFlashLocation;

		//The gameObject that we spawn at the point of the raycasthit. This bullet does't have a mesh renderer and is just a sphere collider tagged Bullet.
		public GameObject FiredBullet;
		//This object gets activated when ever we shoot. It can be detected by ghosts.
		public GameObject Noise;

		//The animator that equips and uniquips the gun.
		private Animator _equippedAnimator;
		//The animator that controlls shooting/reloading.
		public Animator GunAnimator;

		//Is the gun equipped
		private bool _equipped;
		//Are we making Noise?
		private bool Noisy;

		//The raycast layer for the gun.
		public LayerMask GunLayerMask;

		//Audiosources
		public AudioSource gunSound;
		private SoundManager _soundManager;

		private Text _ammoCountHud;


		// Use this for initialization
		void Start () 
		{
			GunAnimator.SetInteger("Shot", 0);
			_equippedAnimator = GetComponent<Animator>();

			_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
			gunSound.clip = _soundManager.GetGunSound(0);

			_ammoCountHud = GameObject.Find("AmmoCount").GetComponent<Text>();
			updateAmmoCountHud();

			EventManager.AddListener((int) GameManagerScript.GameEvents.LoadlastCheckpoint, OnLoadLastCheckpoint);

		}
		
		// Update is called once per frame
		void Update () 
		{
			if(Noisy == true) {
				Noise.SetActive(false);
				Noisy = false;
			}
			if(_equipped && !GameManagerScript.isPaused)
			{
				//If we try to fire, have more than 0 bullets, are not reloading, and our fire rate timer is less than 0.
				//We can fire.
				if (Input.GetButtonDown ("Fire1") && _bullets >= 1 && _reloading == false && _fireRateTimer <= 0f) 
				{
					gunSound.Play();
					Noise.SetActive(true);
					Noisy = true;
					_bullets -= 1;
					updateAmmoCountHud();
					GunAnimator.SetInteger("Shot", GunAnimator.GetInteger("Shot")+1);
					_fireRateTimer = FireRate;

					//Start the muzzle flare
					GameObject muzzleflash = (GameObject)Instantiate(MuzzleFlash, MuzzleFlashLocation.transform.position, MuzzleFlashLocation.transform.rotation);
					muzzleflash.transform.parent = MuzzleFlashLocation.transform;

					//Raycast from the camera.
					RaycastHit hitInfo;
					if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 99999f, GunLayerMask))
					{
						Instantiate(FiredBullet, hitInfo.point, hitInfo.transform.rotation);
						Instantiate(HitSpark, hitInfo.point, hitInfo.transform.rotation);
						Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hitInfo.distance, Color.yellow, .4f);
					}
				}
				
				_fireRateTimer -= Time.deltaTime;

				//If we try to reload and we have less than 6 bullets and aren't already reloading.
				if (Input.GetKeyDown (KeyCode.R) && _bullets != 6 && _reloading == false && reserveAmmo > 0) {
					GunAnimator.SetBool("Reloading", true);
					_reloading = true;
					_reloadTimer = ReloadTime;
					GunAnimator.SetFloat("ReloadTime", ReloadTime);

				}
				//If we try to fire and have no bullets, and are not reloading, we should reload.
				if (Input.GetButtonDown ("Fire1") && _bullets == 0f && _reloading == false && reserveAmmo > 0) {
					GunAnimator.SetBool ("Reloading", true);
					_reloading = true;
					_reloadTimer = ReloadTime;
					GunAnimator.SetFloat("ReloadTime", ReloadTime);
				}
				
				//Decrease timer when gun is reloading.
				if (_reloading == true)
				{
					_reloadTimer -= Time.deltaTime;
					GunAnimator.SetFloat("ReloadTime", _reloadTimer);
				}
				
				//If reloadTime is 0 or less, tell the animator we're done reloading then set reload _timer to 1
				if (_reloadTimer <= 0 && _reloading == true)
				{
					_reloading = false;
					_reloadTimer = 1f;
					GunAnimator.SetInteger("Shot", 0);
					GunAnimator.SetFloat("ReloadTime", 4f);
					GunAnimator.SetBool ("Reloading", false);
					int reloadBullets = -1 * (_bullets - maxBullets);
					//Player has an equivalent or less than required ammount of bullets for full reload
					if(reloadBullets >= reserveAmmo)
					{
						_bullets += reserveAmmo;
						changeReserveAmmo(-reloadBullets);
					}
					else
					{
						_bullets += reloadBullets;
						changeReserveAmmo(-reloadBullets);
					}
					//updateAmmoCountHud();
				}

			}
		}



		public void setEquipped(bool equipped)
		{
			//If the gun is unequipped pause the animations. If it's equipped resume them.
			if(!equipped)
			{
				_equipped = equipped;
				GunAnimator.speed = 0f;
				//GunAnimator.enabled = false;
			}
			else
			{
				_equipped = equipped;
				GunAnimator.speed = 1f;
				//GunAnimator.enabled = true;
			}
			if (_equippedAnimator)	
				_equippedAnimator.SetBool("Equipped", equipped);

		}

		public void changeReserveAmmo(int value)
		{
			reserveAmmo += value;
			if(reserveAmmo < 0)
				reserveAmmo = 0;
				
			updateAmmoCountHud();
		}

		public void updateAmmoCountHud()
		{
			_ammoCountHud.text = _bullets + "/" + reserveAmmo;
		}

		public int GetBulletCount()
		{
			return _bullets;
		}

		public int GetReserveCount()
		{
			return reserveAmmo;
		}

		void OnLoadLastCheckpoint(Component poster, object checkpointData)
		{
			CheckpointClass.Checkpoint.CheckpointData newCheckpointData = (CheckpointClass.Checkpoint.CheckpointData) checkpointData;

			_bullets = newCheckpointData.Bullets;
			reserveAmmo = newCheckpointData.ReserveBullets;
		}
	}
}