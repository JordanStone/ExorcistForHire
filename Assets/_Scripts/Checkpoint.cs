using UnityEngine;
using System.Collections;
using NashTools;
using GunController;


namespace CheckpointClass
{
	public class Checkpoint : MonoBehaviour {

	//	[Header ("These are the respawn attributes")]
	//	[Header ("Don't modify these unless you want to.")]

		[System.Serializable]
		public struct CheckpointData
		{
			public Vector3 SpawnLocation;
			public int Bullets;
			public int ReserveBullets;
			public float Health;
			public GameObject[] BulletBoxes;
			public GameObject[] Enemies;
		}

		private bool _reached = false;


		private CheckpointData _checkpointData;

		private PlayerStats _player;
		private SpiritGun _spiritGun;

		// Use this for initialization
		void Start () 
		{
			_checkpointData.SpawnLocation = GetComponentInChildren<Transform>().position;
			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
			_spiritGun = GameObject.Find("Gun").GetComponent<SpiritGun>();

			_checkpointData.SpawnLocation = transform.GetChild(0).transform.position;

			EventManager.AddListener((int) GameManagerScript.GameEvents.LoadlastCheckpoint, OnLoadLastCheckpoint);
		}


		void OnTriggerEnter(Collider other)
		{
			if(other.tag == "Player" && !_reached)
			{
				//Set reached to true so we can't trigger the checkpoint again.
				_reached = true;

				//Set the checkpoint data.
				_checkpointData.Bullets = _spiritGun.GetBulletCount();
				_checkpointData.ReserveBullets = _spiritGun.GetReserveCount();
				_checkpointData.Health =  _player.GetHealth();

				_checkpointData.BulletBoxes = GameObject.FindGameObjectsWithTag("AmmoBox");
				_checkpointData.Enemies = GameObject.FindGameObjectsWithTag("Enemy");

				//Send the checkpoint data to game manager
				EventManager.PostNotification((int) GameManagerScript.GameEvents.Checkpoint, this, _checkpointData);
			}
		}


		void OnLoadLastCheckpoint(Component poster, object checkpointData)
		{
			CheckpointClass.Checkpoint.CheckpointData newCheckpointData = (CheckpointClass.Checkpoint.CheckpointData) checkpointData;
			

			//Reset all bullet boxes.
			foreach( GameObject BulletBox in newCheckpointData.BulletBoxes)
			{	
				BulletBox.SetActive(true);
				
			}

			foreach( GameObject Enemy in newCheckpointData.Enemies)
			{
				Enemy.SetActive(true);
			}
		}

//



	}

}