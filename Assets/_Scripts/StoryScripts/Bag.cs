using UnityEngine;
using System.Collections;

public class Bag : MonoBehaviour {
	public GameObject[] Contents;
	public GameObject[] Deactivate;
	public GameObject[] Activate;
	void OnCollisionEnter(Collision collision) 
	{
		if(collision.gameObject.tag == "Bullet") 
		{
			GetComponent<Rigidbody>().useGravity = true;
		}
	}
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "BagTrigger") {
			for(int i = 0; i < Contents.Length; i++) {
				Contents[i].SetActive(true);
			}
			for(int i = 0; i < Deactivate.Length; i++) {
				Deactivate[i].SetActive(false);
			}
			for(int i = 0; i < Activate.Length; i++) {
				Activate[i].SetActive(true);
			}
			col.gameObject.SetActive(false);
		}
	}
}
