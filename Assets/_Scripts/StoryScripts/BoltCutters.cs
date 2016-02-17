using UnityEngine;
using System.Collections;

public class BoltCutters : MonoBehaviour {
	public DoorUnlockHandler Handler;
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "EntranceTrigger2") {
			Handler.UnlockDoorWithBoltCutters();
		}
	}
}
