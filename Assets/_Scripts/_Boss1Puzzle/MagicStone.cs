using UnityEngine;
using System.Collections;
using DG.Tweening;
using FinalStone;
using NashTools;

/* This script works in junction with FinalStonePlacement.cs
 * 
 * The Purpose of this script is to allow the player to put 
 * magic stones on the final pedestals to beat the final boss.
 * 
 * Both scripts use a stone type enum to check to see if the right
 * stone is placed on the right pedestal.
 * 
*/

namespace FinalStone
{
	public class MagicStone : MonoBehaviour 
	{

		public ItemType Type = ItemType.NONE;

		private	Vector3 _startingPosition;
		private Quaternion _startingRotation;


		// Use this for initialization
		void Start () 
		{
			EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);
			_startingPosition = transform.position;
			_startingRotation = transform.rotation;
		}
		
		// Update is called once per frame
		void Update () 
		{
			
		}

		void OnReset(Component poster, object checkpointData)
		{
			gameObject.transform.rotation = _startingRotation;
			gameObject.transform.position = _startingPosition;

		}

		public ItemType GetItemType()
		{
			return Type;
		}	

	}
}
