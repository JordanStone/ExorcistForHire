using UnityEngine;
using System.Collections;

public class Fired_Bullet_Handler : MonoBehaviour {

	public GameObject DeathParticles;

	public float fireSpeed;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * fireSpeed, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnDestroy()
	{

	}

	void OnCollisionEnter(Collision collision)
	{
		Instantiate(DeathParticles, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}
}
