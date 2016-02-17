using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NashTools;

public class DoorUnlockHandler : MonoBehaviour {
	public int DoorNumber;
	//public float hingeMax = 120f;
	private HingeJoint _doorHinge;

	private AudioSource _UnlockSound;
//	private JointLimits _doorHingeLimits;


	public GameObject MetalLock;
	private Rigidbody _LockRigidbody;
	private Rigidbody _DoorRigidbody;
	
	// Use this for initialization
	void Start () 
	{
		_doorHinge = GetComponent<HingeJoint>();
//		_doorHingeLimits = _doorHinge.limits;
		
		_doorHinge.useLimits = true;
		
		EventManager.AddListener((int) GameManagerScript.GameEvents.keyPickedUp, OnKeyPickedUp);
		EventManager.AddListener((int) GameManagerScript.GameEvents.UnlockDoor, OnUnlocked);

		_LockRigidbody = MetalLock.GetComponent<Rigidbody> ();
		_DoorRigidbody = GetComponent<Rigidbody> ();

		_UnlockSound = MetalLock.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	
	void OnUnlocked(Component poster, object DoorInfo)
	{
		if((GameObject)DoorInfo == MetalLock)
		{
			MetalLock.tag = "Grabbable";

			UnlockDoor();
		}
		
	}
	void OnKeyPickedUp(Component poster, object UnlockedInfo)
	{
		if((int) UnlockedInfo == DoorNumber)
		{
			MetalLock.tag = (string)"LockWithKeyInInv";
		}
		
	}

	public void UnlockDoor() 
	{

		_UnlockSound.Play ();

		//activate the key animation
		MetalLock.transform.GetChild (0).gameObject.SetActive(true);

		StartCoroutine ("WaitForAnim");
	}

	public void UnlockDoorWithBoltCutters()
	{
		_doorHinge.useLimits = false;
		_DoorRigidbody.isKinematic = false;
		_LockRigidbody.isKinematic = false;
		MetalLock.tag = "Grabbable";

	}

	IEnumerator WaitForAnim()
	{
		yield return new WaitForSeconds(1f);
		MetalLock.transform.GetChild (0).gameObject.SetActive(false);
		_doorHinge.useLimits = false;
		_DoorRigidbody.isKinematic = false;
		_LockRigidbody.isKinematic = false;
		MetalLock.transform.SetParent(null);
	}


}
