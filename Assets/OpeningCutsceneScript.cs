using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using LoadingScreenNamespace;

public class OpeningCutsceneScript : MonoBehaviour {
	
	public Image Fader;
	public Text CutsceneText;
	public LoadingScreen LoadScreen;

	public float imageChangeTime = 1f;
	//public float imageStayTime = 5f;
	
	public Image[] CutsceneImages;
	public string[] CutsceneTexts;
	public float[] CutsceneImageSeconds;
	
	// Use this for initialization
	void Start ()
	{
		foreach( Image myImage in CutsceneImages)
		{
			myImage.enabled = false;
		}

		StartCoroutine("Cutscene");

		Fader.DOFade(0f, .0001f);

	}

	IEnumerator Cutscene()
	{
		for(int i = 0; i < CutsceneImages.Length; i++)
		{
			//Change the text.
			CutsceneText.text = CutsceneTexts[i];
			//Enable current image.
			CutsceneImages[i].enabled = true;
			//Fade out the fader.
			Tween myTween = Fader.DOFade(0f, imageChangeTime);
			//Keep the image shown for X seconds
			yield return new WaitForSeconds(CutsceneImageSeconds[i]);
			//Start the fade.
			Tween myTweenTwo = Fader.DOFade(1f, imageChangeTime);
			yield return myTweenTwo.WaitForCompletion();
			//Switch out the images.
			CutsceneImages[i].enabled = false;
		}

		CutsceneText.text = " ";
		Fader.DOFade(0f, .0001f);
		LoadScreen.LoadLevel("Town_10");
	
	}


	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Pause"))
		{
			StopCoroutine("Cutscene");
			foreach( Image myImage in CutsceneImages)
			{
				myImage.enabled = false;
			}
			CutsceneText.text = " ";
			Fader.DOFade(0f, .0001f);
			LoadScreen.LoadLevel("Town_10");
		}

	}
}
