using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OpenningScript2 : MonoBehaviour {

    public bool timetozoomin;
    public bool timetozoomout;
	public bool timetoguyplay;


	// Use this for initialization
	void Start () {
		timetozoomin=false;
		timetozoomout=false;
		timetoguyplay=false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void timeto(){
		timetozoomin = true;
	}

	public void gunplay(){
		timetoguyplay = true;
	}
	
	public void timeout(){
		timetozoomout=true;
	}


	public void destoryOpen(){
		Destroy(this.gameObject);
	}
}
