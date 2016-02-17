using UnityEngine;
using System.Collections;

public class PlayerTeleport : MonoBehaviour {

	public KeyCode TeleportKey;

	private GameObject _player;

	// Use this for initialization
	void Start () 
	{
		_player = GameObject.Find ("Player");	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (TeleportKey)) 
		{
			_player.transform.position = transform.position;
		}
	}
}
