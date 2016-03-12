using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;


public class ShakeTextOutline : MonoBehaviour {

	private Outline _myOutline;

	public float MaxEffectDistance;
	public float MinEffectDistance;

	public float ShakeTime;
	
	// Use this for initialization
	void Start () 
	{
		_myOutline = GetComponent<Outline>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		StartCoroutine("ShakeCoroutine");
		
		
		
	}
	
	IEnumerator ShakeCoroutine()
	{
		float newEffectDistance = 1f;

		while(true)
		{

			Tween myTween = DOTween.To(()=> newEffectDistance, x=>  newEffectDistance = x, Random.Range(MinEffectDistance,MaxEffectDistance), ShakeTime);
			_myOutline.effectDistance = new Vector2(newEffectDistance, - newEffectDistance);
			yield return myTween.WaitForCompletion();
		}
	}
}

