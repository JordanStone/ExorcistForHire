using UnityEngine;
using System.Collections;
using SoundController;

public class bumpSound : MonoBehaviour {

	public float minVel = 2f;
	public float midVel = 5f;
	public float highVel = 10f;

	Rigidbody _myBody;
	Vector3 _myVelocity;

	private AudioSource source;

	// Use this for initialization
	void Start () {
		_myBody = GetComponent<Rigidbody>();
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

//		print("UPDATE" + _velocity.x);
//		print("UPDATE" +_velocity.y);
//			print("UPDATE" +_velocity.z);	
	}

	void LateUpdate(){
		_myVelocity = _myBody.velocity;

	}

	void OnCollisionEnter(){
//		print ("ow");
//		Vector3 _myVelocity = _myBody.velocity;
//		print(_myVelocity.x);
//		print(_myVelocity.y);
//		print(_myVelocity.z);
		if (Mathf.Abs(_myVelocity.x) >= highVel || Mathf.Abs(_myVelocity.y) >= highVel || Mathf.Abs(_myVelocity.z) >= highVel)
		{
			if (source != null && !source.isPlaying)
				source.PlayOneShot(source.clip,1f);	
			print("HIGH");
		}
		else if (Mathf.Abs(_myVelocity.x) >= midVel || Mathf.Abs(_myVelocity.y) >= midVel || Mathf.Abs(_myVelocity.z) >= midVel)
		{
			if (source != null && !source.isPlaying)
				source.PlayOneShot(source.clip,0.7f);	
			print("MID");
		}
		else if (Mathf.Abs(_myVelocity.x) >= minVel || Mathf.Abs(_myVelocity.y) >= minVel || Mathf.Abs(_myVelocity.z) >= minVel)
		{
			if (source != null && !source.isPlaying)
				source.PlayOneShot(source.clip,0.2f);	
			print("LOW");
		}
	}
}
