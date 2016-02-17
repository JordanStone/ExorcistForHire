using UnityEngine;
using System.Collections;
using FinalStone;
using NashTools;

public class ForceField : MonoBehaviour {


	//The number of stones need to be placed. Five is the default.
	//It's public for testing/possible gameplay change purposes.
	public int StonesNumber = 5;

	//How many stones are down!
	private int _stonesPlaced = 0;


	//Rotation speeds
	public float XRotSpeed;
	public float YRotSpeed;
	public float ZRotSpeed;

	private Vector3 _startingPosition;

	// Use this for initialization
	void Start () 
	{
		//Listen for stones to be down.
		EventManager.AddListener((int) GameManagerScript.GameEvents.StonePlaced, OnStonePlaced);
		EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);
		_startingPosition = transform.position;
	}

	void Update()
	{
		//Rotate the forcefield because it looks cool I guess!
	//	transform.Rotate(new Vector3 (Random.Range (0f, XRotSpeed), Random.Range (0f, YRotSpeed), Random.Range (0f, ZRotSpeed)));
		transform.Rotate(new Vector3 (XRotSpeed, YRotSpeed,  ZRotSpeed) );
	}

	//When a stone is placed incriment _stonesPlaced by 1
	//Then check to see if _stonesPlaced is StonesNumber. If it is we can get rid of the forcefield... Which is this object.
	void OnStonePlaced(Component poster, object incrimentNumber)
	{
		_stonesPlaced++;

		if (_stonesPlaced == StonesNumber) 
		{
			transform.position = new Vector3 (0f, -99999999f, 0f);
		}

	}
	void OnReset(Component poster, object incrimentNumber)
	{

		_stonesPlaced = 0;
		transform.position = _startingPosition;
	}


}
