using UnityEngine;
using System.Collections;
using NashTools;
using SoundController;

public class AmmoBoxHandler : MonoBehaviour 
{
	private SoundManager _soundManager;
	private AudioClip collectSound;

	private GunController.SpiritGun _spiritGun;
	public int value;

	// Use this for initialization
	void Start () 
	{
		_spiritGun = GameObject.Find("Gun").GetComponent<GunController.SpiritGun>();
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		collectSound = _soundManager.GetSFXSound(2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable()
	{		
		if (collectSound)
		{
			AudioSource.PlayClipAtPoint(collectSound, transform.position);
		}
		_spiritGun.changeReserveAmmo(value);
	}
}
