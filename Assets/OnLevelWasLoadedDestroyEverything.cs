using UnityEngine;
using System.Collections;

public class OnLevelWasLoadedDestroyEverything : MonoBehaviour {

	void OnLevelWasLoaded(int level) {
		if (level == 3)
			Destroy (gameObject);
	}
	
	
}
