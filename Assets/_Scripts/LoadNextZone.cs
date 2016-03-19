using UnityEngine;
using System.Collections;
using LoadingScreenNamespace;
using NashTools;


public class LoadNextZone : MonoBehaviour {

	private LoadingScreen _loadingScreen;
	// Use this for initialization
	void Start () 
	{
		_loadingScreen = GameObject.Find("Loading").GetComponent<LoadingScreen>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) 
	{
		if(other.name == "Player")
		{
//		_loadingScreen.LoadLevel("Catacombs2");

			Application.LoadLevel("Catacombs2");
		}
	
	}
}
