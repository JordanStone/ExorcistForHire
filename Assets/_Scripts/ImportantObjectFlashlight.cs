using UnityEngine;
using System.Collections;
using DG.Tweening;


public class ImportantObjectFlashlight : MonoBehaviour {


	public Color Color1;
	public float ChangeTime;

	private Light _light;

	private Sequence mySequence;

	// Use this for initialization
	void Start () 
	{
		_light = GetComponent<Light>();

		_light.DOColor(Color1, ChangeTime).SetLoops(-1, LoopType.Yoyo);
	}


	// Update is called once per frame
	void Update () 
	{

	
	}
}
