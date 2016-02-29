using UnityEngine;
using System.Collections;
using BossDetectionScript;

public class BossAddBehavior : MonoBehaviour {
	private DarkGodStateMachine _bossDetectionScript;
	private LineRenderer bossTether;
	public GameObject BossToTrack;
	private Vector3 offsetTether;
	public int lengthOfTether = 5;
	//private Vector3 bossOffsetTether;

	// Use this for initialization
	void Start () {
		_bossDetectionScript = GameObject.Find("DarkGod").GetComponent<DarkGodStateMachine>();
		bossTether = GetComponent<LineRenderer>(); //Used to draw visual tether between cultists and boss
		offsetTether = new Vector3(0f, 1f, 0f); //Offsets tether height
		bossTether.SetWidth(0.2f, 0.2f); //Width of tether to boss and their shield
		bossTether.SetVertexCount(lengthOfTether); //sets the number of vertices of the line (Used for sine function)
		//bossOffsetTether = new Vector3(0f, 1f, 0f);

	}
	

	void Update()
	{
		float t = Time.time;

		bossTether.SetPosition(0, transform.position + offsetTether); //Start position of tether (cultist)
		for(int i=1; i<(lengthOfTether -1); i++)
		{
			Vector3 pos = Vector3.Lerp(transform.position + offsetTether, BossToTrack.transform.position + offsetTether, (float)i/lengthOfTether); //Interpolate to vertices in between start and end points

			pos += new Vector3(0f, Mathf.Sin(i + t), 0f); //Sine function makes tether behave like sine wave
			
			bossTether.SetPosition(i, pos);
		}
		bossTether.SetPosition((lengthOfTether -1), BossToTrack.transform.position + offsetTether); //End position of tether (boss)
	}

	public void OnDisable()
	{
		_bossDetectionScript.OnBossAddDeath();
		Destroy(bossTether);
	}

	public void AddDeath()
	{
		_bossDetectionScript.OnBossAddDeath();
		Destroy(bossTether);
	}
}
