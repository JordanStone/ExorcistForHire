using UnityEngine;
using System.Collections;
using NashTools;


public class AmmoBoxHandler : MonoBehaviour 
{
	private GunController.SpiritGun _spiritGun;
	public int value;

	// Use this for initialization
	void Start () 
	{
		_spiritGun = GameObject.Find("Gun").GetComponent<GunController.SpiritGun>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable()
	{
		_spiritGun.changeReserveAmmo(value);
	}
}
