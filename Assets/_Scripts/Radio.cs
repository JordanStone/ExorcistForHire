using UnityEngine;
using System.Collections;
using NashTools;


public class Radio : MonoBehaviour 
{
	private GameObject player;
	
	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("MainCamera");
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnDisable()
	{
		player.GetComponent<skyboxRotation>().day=false;
	}
}
