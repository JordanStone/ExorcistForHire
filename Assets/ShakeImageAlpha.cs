using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ShakeImageAlpha : MonoBehaviour {

	public float ShakeIntensity;

	private Image _myImage;

	// Use this for initialization
	void Start () 
	{
		_myImage = GetComponent<Image>();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		StartCoroutine("ShakeCoroutine");



	}

	IEnumerator ShakeCoroutine()
	{
		while(true)
		{
			Tween myTween = _myImage.DOFade(Random.Range(.6f, 1f), ShakeIntensity);
			yield return myTween.WaitForCompletion();
		}
	}
}
