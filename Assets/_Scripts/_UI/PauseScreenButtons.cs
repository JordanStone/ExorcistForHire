using UnityEngine;
using System.Collections;
using NashTools;
using UnityEngine.UI;

public class PauseScreenButtons : MonoBehaviour {

	public Canvas MyCanvas;
	public GameObject options;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void QuitButton()
	{
		Application.Quit();
	}

	public void RestartButton()
	{
		Application.Quit();
		EventManager.Cleanup();
		MyCanvas.enabled = true;

		Application.LoadLevel(Application.loadedLevel);

	}

	public void ReturnToCheckpoint()
	{
		EventManager.PostNotification((int) GameManagerScript.GameEvents.LoadLastCheckpointPressed, this, null);

	}

	public void Option()
	{

		options.GetComponent<_OptionMenuListener>().setImageTo(true);
	}


}
