using UnityEngine;
using System.Collections;

public class DestroyPSOnFinish : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void LateUpdate()
	{
		if (GetComponent<ParticleSystem>().particleCount == 0f && GetComponent<ParticleSystem>().isPlaying == false )
		{
			if(gameObject.transform.parent != null && gameObject.transform.parent.name != "MuzzleFlash")
			{
				Destroy(gameObject.transform.parent.gameObject);
			}
			Destroy(gameObject);
		}
	}
}
