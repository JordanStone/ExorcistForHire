using UnityEngine;
using System.Collections;

public class OtakuExplosion : MonoBehaviour {

	private GameObject _player;
	private bool _active = false;

	private AudioSource spookNoise;


	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < this.transform.GetChildCount(); ++i)
		{
			this.transform.GetChild(i).gameObject.SetActive(false);
		}
	
		_player = GameObject.Find ("Player");


		spookNoise = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider other) {
		if(other.transform.gameObject == _player && _active == false)
		{
			_active = true;
			spookNoise.Play();
			
			for (int i = 0; i < this.transform.GetChildCount(); ++i)
			{
				this.transform.GetChild(i).gameObject.SetActive(true);
			}

		}
	}

}
