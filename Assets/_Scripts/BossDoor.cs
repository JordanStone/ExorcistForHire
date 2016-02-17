using UnityEngine;
using System.Collections;
using NashTools;

public class BossDoor : MonoBehaviour {

	public GameObject BossDoorOne;
	public GameObject BossDoorTwo;

	private Vector3 _doorOnePosition;
	private Vector3 _doorTwoPosition;

	private JointLimits _lockedLimits;

	public ParticleSystem BlackSmoke;
	private float _smokeTimer;
	public  float DeathTime;

	private bool _bossStart = false;

	private GameObject[] _DeathGhosts;
	private int _stonesPlaced;
	public int StonesNumber = 5;

	public float MaxEmission = 10f;

	public AnimationCurve ParticleCurve;

	// Use this for initialization
	void Start () 
	{
		BlackSmoke.emissionRate = 0f;

		_doorOnePosition = BossDoorOne.transform.position;
		_doorTwoPosition = BossDoorTwo.transform.position;

		EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);
		EventManager.AddListener((int) GameManagerScript.GameEvents.StonePlaced, OnStonePlaced);

		if(_DeathGhosts == null)
			_DeathGhosts = GameObject.FindGameObjectsWithTag("FinalBossGhost");

		foreach (GameObject _DeathGhost in _DeathGhosts)
		{
			_DeathGhost.SetActive(false);
			
		}
	}


	//	_smokeTimer/DeathTime = percentage till death.
	//  MaxEmission = max particles on screen.
	// _smokeTimer/DeathTime * MaxEmission = Correct Emmision.


	// Update is called once per frame
	void Update () 
	{
		if (_bossStart) 
		{
			//SmokeStarts
			_smokeTimer += Time.deltaTime;
			BlackSmoke.emissionRate = _smokeTimer/DeathTime*MaxEmission;

			if(_smokeTimer >= DeathTime)
			{

				//kill the player
				//EventManager.PostNotification((int) GameManagerScript.GameEvents.Dead, this, true);

				//Activate all the invincible deathghosts
				if(_DeathGhosts == null)
					_DeathGhosts = GameObject.FindGameObjectsWithTag("FinalBossGhost");

				foreach (GameObject _DeathGhost in _DeathGhosts)
				{
					_DeathGhost.SetActive(true);

				}
			}

		}
	}

	void OnReset(Component poster, object checkpointData)
	{
		BossDoorOne.transform.eulerAngles = Vector3.zero;
		BossDoorTwo.transform.eulerAngles = Vector3.zero;
		
		BossDoorOne.transform.position = _doorOnePosition;
		BossDoorTwo.transform.position = _doorTwoPosition;
		
		BossDoorOne.GetComponent<Rigidbody>().isKinematic = false;
		BossDoorTwo.GetComponent<Rigidbody>().isKinematic = false;

		_smokeTimer = 0f;
		_bossStart = false;
		BlackSmoke.emissionRate = 0f;

		if(_DeathGhosts == null)
			_DeathGhosts = GameObject.FindGameObjectsWithTag("FinalBossGhost");
		
		foreach (GameObject _DeathGhost in _DeathGhosts)
		{
			_DeathGhost.SetActive(false);
			
		}
		_stonesPlaced = 0;

	}
	
	//If an object enters the trigger.
	void OnTriggerEnter(Collider other) 
	{
		//If the object is the player
		if (other.gameObject.tag == "Player") 
		{
			_bossStart = true;

			BossDoorOne.transform.eulerAngles = Vector3.zero;
			BossDoorTwo.transform.eulerAngles = Vector3.zero;

			BossDoorOne.transform.position = _doorOnePosition;
			BossDoorTwo.transform.position = _doorTwoPosition;
			
			BossDoorOne.GetComponent<Rigidbody>().isKinematic = true;
			BossDoorTwo.GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	void OnStonePlaced(Component poster, object incrimentNumber)
	{
		_stonesPlaced++;
		
		if (_stonesPlaced == StonesNumber) 
		{
			_bossStart = false;
			_smokeTimer = 0f;
			BlackSmoke.emissionRate = 0f;
		}
		
	}

}
