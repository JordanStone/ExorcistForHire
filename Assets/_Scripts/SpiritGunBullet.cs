using UnityEngine;
using System.Collections;

public class SpiritGunBullet : MonoBehaviour {


	private int _deathFrames = 1; 

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		_deathFrames -= 1;

		if (_deathFrames <= 0)
		{
			Destroy(gameObject);

		}
	}

	void OnCollisionEnter(Collision collision) 
	{
		Destroy(gameObject);
	}
}
