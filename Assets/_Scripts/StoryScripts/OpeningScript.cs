using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OpeningScript : MonoBehaviour {

	private GameObject animtraiger;
	private bool zoomin;

	// Use this for initialization
	void Start () {
	
		zoomin=false;
		animtraiger = GameObject.Find("OpeningSceneTriger");
	}
	
	// Update is called once per frame
	void Update () {


		if(GameObject.Find("OpeningSceneTriger")) {



			transform.LookAt(animtraiger.transform);
		
		
			if(animtraiger.GetComponent<OpenningScript2>().timetozoomin){
				this.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.gameObject.GetComponent<Camera>().fieldOfView,10,Time.deltaTime);
			}
			
			if(animtraiger.GetComponent<OpenningScript2>().timetozoomout){
				this.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.gameObject.GetComponent<Camera>().fieldOfView,60,Time.deltaTime);
			}

			if(animtraiger.GetComponent<OpenningScript2>().timetoguyplay){
				//add gun animation here
				//add conversation here
			}
		
		
		
		
		}


	
	}
}
