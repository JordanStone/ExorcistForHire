using UnityEngine;
using System.Collections;

public class SpawnEnemiesOnEnter : MonoBehaviour {

	public GameObject[] Enemies;


	private bool _reached = false;

	// Use this for initialization
	void Start () 
	{
		foreach(GameObject enemy in Enemies)
		{
			enemy.SetActive(false);
		}
	}

		
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player" && ! _reached)
		{
			//Set reached to true so we can't trigger the checkpoint again.
			_reached = true;
		
			foreach(GameObject enemy in Enemies)
			{
				enemy.SetActive(true);
			}
		}

	}

}
