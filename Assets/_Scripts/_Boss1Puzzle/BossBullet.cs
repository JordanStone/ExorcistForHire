using UnityEngine;
using System.Collections;

public class BossBullet : MonoBehaviour {


	public float damage=0;
	Vector3 v;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position+=v;


	}

	void OnTriggerEnter(Collider col){



		if(col.transform.tag=="Player"&&col.transform.name=="Player"){
			col.gameObject.GetComponent<PlayerStats>().Gethit(damage);
		}


		Destroy(this.gameObject);

	}
}
