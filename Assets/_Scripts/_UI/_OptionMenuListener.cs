using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NashTools;
using SoundController;

public class _OptionMenuListener : MonoBehaviour {

	[Header ("Check if Not Hidden at Start")]
	public bool ShownAtStart;
	
	[Header("Check if you want to disable all children of this game object when you hide this object")]
	public bool DisableChildren;
	
	private Image _myImage;
	

	void Start () 
	{
		_myImage =gameObject.GetComponent<Image>();	
		
		if(ShownAtStart)
		{
			setImageTo(true);
		}
		else
		{
			setImageTo(false);
		}
		
		EventManager.AddListener((int) GameManagerScript.GameEvents.Paused, OnPaused);		
	}
	
	
	void OnPaused(Component poster, object pausedState)
	{
		if((bool) pausedState != ShownAtStart)
		{


		}
		else
		{

			setImageTo(false);
		}
	}
	
	public void setImageTo(bool state)
	{
		Image _myImage = gameObject.GetComponent<Image>();	
		_myImage.enabled = state;

		if(DisableChildren)
		{
			foreach (Transform child in transform)     
			{  
				child.gameObject.SetActive(state);
			}  
		}
	}
	
	// Button Functions
	// Restarts current scene
	public void Restart(){
		Application.LoadLevel(Application.loadedLevel);
	}
	
	// Quits application
	public void Quit(){
		Application.Quit();
	}
}
