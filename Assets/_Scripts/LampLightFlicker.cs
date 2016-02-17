using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LampLightFlicker : MonoBehaviour 
{
	
	public float minFlickerIntensity = 0.5f;
	public float maxFlickerIntensity = 2.5f;


	public float flickerSpeed  = 0.035f;
	public float returnTime = 0.1f;

	public float maxFlickerSpeed = .1f;
	public float minFlickerSpeed = .035f;

	private float startingIntensity;

	private float randomizer = 0f;

	private	Light _myLight;

	void Start()
	{
		_myLight = GetComponent<Light>();

		startingIntensity = _myLight.intensity;

		StartCoroutine(Flicker());
	}

	IEnumerator Flicker()
	{

		while (true)
		{

		//	_myLight.intensity = (Random.Range (minFlickerIntensity, maxFlickerIntensity));			
		
			returnTime = Random.Range(maxFlickerSpeed, minFlickerSpeed);
			Tween myTween = DOTween.To (()=> _myLight.intensity, x=> _myLight.intensity =x, startingIntensity, returnTime);
			yield return new WaitForSeconds(returnTime);

			flickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);

			myTween = DOTween.To (()=> _myLight.intensity, x=> _myLight.intensity =x, Random.Range(minFlickerIntensity, maxFlickerIntensity), flickerSpeed);
			yield return new WaitForSeconds(flickerSpeed);
		
		}
	}
}