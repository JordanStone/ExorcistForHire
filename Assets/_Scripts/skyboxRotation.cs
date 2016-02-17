using UnityEngine;
using System.Collections;

public class skyboxRotation : MonoBehaviour {


	public Material skybox;
	public Material skybox_night;
	public float rotation_speed;
	public bool day;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {

		if(day){RenderSettings.skybox=skybox;}
		if(!day){RenderSettings.skybox=skybox_night;}

		RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotation_speed);
	}
}
