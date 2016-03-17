using UnityEngine;
using System.Collections;
using NashTools;

public class BossBullet : MonoBehaviour {


	public float damage=0;
	public float thrust = 100f;
	Vector3 v;
	private Transform player;
	Rigidbody rb;


	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player").transform;
		Vector3 target = player.transform.position + new Vector3(0f, 1f, 0f);
		transform.LookAt(target);

		rb = GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * thrust);

	}
	
	// Update is called once per frame
	void Update () {
		//this.gameObject.transform.position+=v;
		//rb.AddForce(transform.forward * thrust);


	}

	void OnCollisionEnter(Collision col){



		if(col.transform.tag=="Player"){
			EventManager.PostNotification((int) GameManagerScript.GameEvents.DamagePlayer, this, damage);
		}

		if(col.transform.tag!="Enemy")
			Destroy(this.gameObject);

	}

}
