using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Bird : MonoBehaviour {
	
	public float alarmdistance = 5f;
	public float flyroationr = 7f;
	
	private float distance;
	private GameObject Player;
	
	private float ori_x,ori_y,ori_z;
	private float new_x,new_y,new_z;
	private float dir_x,dir_z;
	
	
	// Use this for initialization
	void Start () {
		
		Player = GameObject.FindGameObjectWithTag("Player");
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//the distance between player and bird
		distance = Vector3.Distance(Player.transform.position,this.transform.position);
		
		if(distance<=alarmdistance){
			//get the original position before fly
			Vector3 ori_position = this.transform.position;
			ori_x = this.transform.position.x;
			ori_y = this.transform.position.y;
			ori_z = this.transform.position.z;
			
			//fly against the player closing direction
			dir_x = -(Player.transform.position.x-this.transform.position.x);
			dir_z = -(Player.transform.position.z-this.transform.position.z);
			
			//set new fly to position
			new_x  = Random.Range(5f, 8f)*dir_x;
			new_y  = Random.Range(10f, 12f);
			new_z  = Random.Range(5f, 8f)*dir_z;
			
			//set fly around position[]
			Vector3[] path = new Vector3[9];
			Vector3[] path2 = new Vector3[9];
			Vector3 skypositon = new Vector3(ori_x+new_x,ori_y+new_y,ori_z+new_z);
			
			float temp = Mathf.Sqrt((flyroationr*flyroationr)/2);
			
			path[0] = skypositon;
			path[1] = skypositon + new Vector3(flyroationr-temp,0,temp);
			path[2] = skypositon + new Vector3(flyroationr,0,flyroationr);
			path[3] = skypositon + new Vector3(temp+flyroationr,0,flyroationr);
			path[4] = skypositon + new Vector3(2*flyroationr,0,0);
			path[5] = skypositon + new Vector3(temp+flyroationr,0,-temp);
			path[6] = skypositon + new Vector3(flyroationr,0,-flyroationr);
			path[7] = skypositon + new Vector3(flyroationr-temp,0,-temp);
			path[8] = ori_position;
			
			path2[0] = skypositon;
			path2[1] = skypositon + new Vector3(flyroationr-temp,0,-temp);
			path2[2] = skypositon + new Vector3(flyroationr,0,-flyroationr);
			path2[3] = skypositon + new Vector3(temp+flyroationr,0,-temp);
			path2[4] = skypositon + new Vector3(2*flyroationr,0,0);
			path2[5] = skypositon + new Vector3(temp+flyroationr,0,flyroationr);
			path2[6] = skypositon + new Vector3(flyroationr,0,flyroationr);
			path2[7] = skypositon + new Vector3(flyroationr-temp,0,temp);
			path2[8] = ori_position;
			
			float ran = Random.Range(-5f, 5f);
			
			//set action sequence
			Sequence mySequence = DOTween.Sequence();
			
			
			//moving around
			if(ran>0){
				mySequence.Append(transform.DOPath(path,Random.Range(15f, 25f),PathType.CatmullRom,PathMode.Sidescroller2D).SetLookAt(0.5f));}
			else{
				mySequence.Append(transform.DOPath(path2,Random.Range(15f, 25f),PathType.CatmullRom,PathMode.Sidescroller2D).SetLookAt(0.5f));}
			
			mySequence.Append(transform.DOLocalRotate (new Vector3 (0f,0f, 0f), 0.1f));
		}
	}
}