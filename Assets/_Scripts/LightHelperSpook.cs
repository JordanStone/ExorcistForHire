using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LightHelperSpook : MonoBehaviour {


//	public	Light[] Lights;

	public Light Light1;
	public Light Light2;
	public Light Light3;


	private Sequence mySequence;
	public Color Color1;
	public float ChangeTime;
	public float minRange;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "Player")
		{
			/*
			foreach( Light cielingLight in Lights)
			{
				cielingLight.DOColor(Color1, ChangeTime);

				mySequence.Insert(0, DOTween.To (()=> cielingLight.range, x=> cielingLight.range =x, minRange, ChangeTime));
			}

			*/

			Light1.DOColor(Color1, ChangeTime);
			Light2.DOColor(Color1, ChangeTime);
			Light3.DOColor(Color1, ChangeTime);

			DOTween.To (()=> Light1.range, x=> Light1.range =x, minRange, ChangeTime);
			DOTween.To (()=> Light2.range, x=> Light2.range =x, minRange, ChangeTime);			
			DOTween.To (()=> Light3.range, x=> Light3.range =x, minRange, ChangeTime);


		}
	

	}
}
