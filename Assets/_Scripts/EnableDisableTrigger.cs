using UnityEngine;
using System.Collections;

public class EnableDisableTrigger : MonoBehaviour {
	public GameObject[] Enable;
	public GameObject[] Disable;
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "Player") {
			foreach(GameObject g in Enable) {
				g.SetActive(true);
			}
			foreach(GameObject g in Disable) {
				g.SetActive(false);
			}
		}
	}
}
