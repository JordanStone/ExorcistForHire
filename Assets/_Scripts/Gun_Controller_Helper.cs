using UnityEngine;
using System.Collections;

public class Gun_Controller_Helper : MonoBehaviour {

	public GameObject Gun_ControllerObject;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DoneFiring()
	{
		Gun_ControllerObject.SendMessage("DoneFiring");
	}

	void DoneReloading()
	{
		Gun_ControllerObject.SendMessage("DoneReloading");
	}
	void PauseFire()
	{
		Gun_ControllerObject.SendMessage("PauseFire");
	}


}