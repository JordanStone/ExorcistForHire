using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NashTools;
using LoadingScreenNamespace;


//This script show's and hides what it's attached to when the game is paused.

public class _ImageMainMenuListener : MonoBehaviour {
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
		
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}


	public void QuitButton()
	{
		Application.Quit();
	}

	void setImageTo(bool state)
	{
		_myImage.enabled = state;
			
		if(DisableChildren)
		{
			foreach (Transform child in transform)     
			{  
				child.gameObject.SetActive(state);
			}  
		}
	}
}