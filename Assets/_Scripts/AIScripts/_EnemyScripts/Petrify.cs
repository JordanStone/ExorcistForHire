using UnityEngine;
using System.Collections;

// Add to object created with boss's petrify attack. If player looks at this object, they get stoned, yo
public class Petrify : MonoBehaviour {

	public float forwardRayDistance = 2.5f;
	public float petrifyTime = 2f;
	public float cubeExistTime = 4f;
	private float timer = 0;

	private GameObject player;
	private GameObject playerCam;

	private RaycastHit hit;

	void Start () {
		player = GameObject.Find("Player");
		StartCoroutine(KillMe(cubeExistTime));
	}

	void Update()
	{
		Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * forwardRayDistance, Color.red, 1);
	}
	
	void FixedUpdate () {
		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, forwardRayDistance) && hit.transform.name == transform.name)
		{
			//The player is looking at us, let's petrify em
//			GetComponent<Renderer>().material.SetColor("_Color",Color.red);
			player.GetComponent<FPSController>().enabled = false;

		}
		else if(player.GetComponent<FPSController>().enabled == false)
		{
			// Player is frozen, make them wait to become unfrozen.
//			GetComponent<Renderer>().material.SetColor("_Color",Color.white);

			timer += Time.deltaTime;

			if (timer >= petrifyTime)
			{
				player.GetComponent<FPSController>().enabled = true;
				timer = 0;
				print("You're free now");
			}
		}	
	}

	IEnumerator KillMe(float time)
	{
		yield return new WaitForSeconds(time);
		print("bye");
		Destroy(gameObject);
	}
}
