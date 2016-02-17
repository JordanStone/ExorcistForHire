using UnityEngine;
using System.Collections;

public class GateTriggerTest : MonoBehaviour {
	public GameObject manager;
	public int GateNumber = 1;

	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "Player") {
			manager.GetComponent<GameManagerScript>().setGate(GateNumber);
		}
	}
}
