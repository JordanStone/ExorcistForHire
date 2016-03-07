using UnityEngine;
using System.Collections;
using NashTools;
using UnityEngine.UI;
using SoundController;

/**
 * Pause Menu Controller
 * Created by Ruben Guzman
 * last updated: 3/4/2015 Ruben Guzman
 * 
 * When "p" or "esc" are pressed, this script enables the pause menu UI.
 * It displays Settings(sound and mouse options), reload checkpoint, and quit buttons.
 * Pressing the settings button plays and animation which "pushes" the settings button to the side,
 * disables it, and enables the sound and mouse option buttons. Pressing reload or quit disables the sound and mouse 
 * options and enables the question for the user if they are sure they would like to reload or quit.
 * 
 * This script also controls the UI for the inventory system. To enable the invetory UI, enable the inventory_but in the 
 * hierarchy(inventory&SettingsCanvas > leftSide > inv_set_buttons > inventory_but) and uncomment out the section in 
 * the PauseGame() method in this script.
 * 
 * */

public class pauseMenuController : MonoBehaviour {

	public GameObject settingsBut;
	public GameObject inventoryBut;
	public GameObject itemsBut;
	public GameObject upgradesBut;
	public GameObject soundBut;
	public GameObject mouseBut;
	public GameObject reloadBut;
	public GameObject quitBut;
	public GameObject yesReloadBut;
	public GameObject yesQuitBut;
	//public Canvas backgroundCanvas;
	public Canvas invSetCanvas;
	public GameObject invSetPanel;
	public GameObject itemsUpPanel;
	public GameObject settingsPanel;
	public GameObject soundSettingsPanel;
	public GameObject mouseSettingsPanel;
	public GameObject reloadSettingsPanel;
	public GameObject quitSettingsPanel;
	public GameObject inventoryItems;
	public bool isPaused;
	bool settingsIsClicked;
	bool inventoryIsClicked;
	//bool itemsIsClicked;
	//bool upgradesIsClicked;
	bool soundIsClicked;
	bool mouseIsClicked;
	bool reloadIsClicked;
	bool quitIsClicked;
	private AudioSource[] audioSources;

	void Awake()
	{
		audioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[]; 
	}
	// Use this for initialization
	//Every object is disabled
	void Start () 
	{
		isPaused = false;
		//backgroundCanvas.enabled = false;
		invSetCanvas.enabled = false;
		settingsIsClicked = false;
		inventoryIsClicked = true;
		//itemsIsClicked = false;
		//upgradesIsClicked = false;
		soundIsClicked = false;
		mouseIsClicked = false;
		reloadIsClicked = false;
		quitIsClicked = false;
		inventoryItems.SetActive (false);
		itemsBut.SetActive (false);
		upgradesBut.SetActive (false);
		settingsPanel.SetActive (false);
		soundSettingsPanel.SetActive (false);
		mouseSettingsPanel.SetActive (false);
		reloadSettingsPanel.SetActive (false);
		quitSettingsPanel.SetActive (false);
		//soundPanel.SetActive (false);
		this.inventoryOnMouseExit ();
		this.settingsOnMouseExit ();
		this.reloadOnMouseExit ();
		this.quitOnMouseExit ();
	}

	//start of pause game
	public void PauseGame()
	{
		if (isPaused == false) 
		{
			isPaused = !isPaused;
			Time.timeScale = 0;
			//backgroundCanvas.enabled = true;
			invSetCanvas.enabled = true;
			invSetPanel.GetComponent<Animator> ().Play ("pause_start");

			//Uncomment this section to enable the inventory system UI
			//inventoryBut.GetComponent<Animator> ().Play ("inventory_highlighted");
			//inventoryItems.SetActive (true);
			//inventoryItems.GetComponent<Animator> ().Play ("inventory_show");
		} 
		else 
		{
			Time.timeScale = 1;
			invSetPanel.GetComponent<Animator> ().Play ("pause_default");
			this.Start ();
		}
	}

	//animation for hovering over inventory button
	public void inventoryOnMouseOver()
	{
		if (inventoryIsClicked == false) 
		{
			inventoryBut.GetComponent<Animator> ().Play ("inventory_highlighted");
		}
	}

	//animation for clicking inventory button
	//opens notes and items panel
	public void inventoryOnMouseDown()
	{
		if (inventoryIsClicked == false)
		{
			if (settingsIsClicked == true)
			{
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked_reversed");
				settingsPanel.GetComponent<Animator> ().Play ("settings_panel_exit");
				settingsIsClicked = false;
			}
			if (quitIsClicked == true) 
			{
				quitSettingsPanel.GetComponent<Animator>().Play("quit_buttons_exit");
				quitIsClicked = false;
			}
			if (reloadIsClicked == true) 
			{
				reloadSettingsPanel.GetComponent<Animator> ().Play ("reload_buttons_exit");
				reloadIsClicked = false;
			}
			inventoryBut.GetComponent<Animator> ().Play ("inventory_highlighted");
			//itemsIsClicked = false;
			//upgradesIsClicked = false;
			//itemsBut.SetActive (true);
			//upgradesBut.SetActive (true);
			inventoryIsClicked = true;
			mouseIsClicked = false;
			soundIsClicked = false;
			//soundPanel.SetActive (false);
			//itemsUpPanel.GetComponent<Animator> ().Play ("inventory_clicked");
			this.reloadOnMouseExit ();
			this.quitOnMouseExit ();
			this.settingsOnMouseExit ();
			//this.notesOnMouseExit ();
			//this.upgradesOnMouseExit ();
			inventoryItems.SetActive (true);
			mouseSettingsPanel.GetComponent<Animator> ().Play ("mouse_settings_exit");
			soundSettingsPanel.GetComponent<Animator> ().Play ("sound_settings_exit");
			inventoryItems.GetComponent<Animator> ().Play ("inventory_show");
			this.soundOnMouseExit ();
			this.mouseOnMouseExit ();
		}
	}

	//animation for hovering out inventory button(default state)
	public void inventoryOnMouseExit()
	{
		if ((inventoryIsClicked == false)) 
		{
			inventoryBut.GetComponent<Animator> ().Play ("inventory_normal");
		}
	}

	//animation for hovering over settings button
	public void settingsOnMouseOver()
	{
		if (settingsIsClicked == false)
		{
			settingsBut.GetComponent<Animator> ().Play ("settings_highlighted");
		}
	}

	//animation for hovering out settings button(default state)
	public void settingsOnMouseExit()
	{
		if (settingsIsClicked == false) 
		{
			settingsBut.GetComponent<Animator> ().Play ("settings_normal");
		}
	}

	//animation for clicking settings button
	//starts sound button and mouse buttons
	public void settingsOnMouseDown()
	{
		if (settingsIsClicked == false)
		{
			if (inventoryIsClicked == true) {
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked");			
				inventoryItems.GetComponent<Animator> ().Play ("upgrades_hideInventory");
				inventoryIsClicked = false;
			}
			if (reloadIsClicked == true) {
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked");
				reloadSettingsPanel.GetComponent<Animator>().Play("reload_buttons_exit");
				reloadIsClicked = false;
			}
			if (quitIsClicked == true) {
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked");			
				quitSettingsPanel.GetComponent<Animator>().Play("quit_buttons_exit");
				quitIsClicked = false;
			}
			settingsBut.GetComponent<Animator> ().Play ("settings_highlighted");
			//itemsUpPanel.GetComponent<Animator> ().Play ("settings_clicked");
			//itemsIsClicked = false;
			//upgradesIsClicked = false;
			settingsIsClicked = true;
			settingsPanel.SetActive (true);
			soundSettingsPanel.SetActive (false);
			mouseSettingsPanel.SetActive (false);
			//this.inventoryOnMouseExit ();
			this.reloadOnMouseExit ();
			this.quitOnMouseExit ();
			this.soundOnMouseExit ();
			this.mouseOnMouseExit ();
			//this.notesOnMouseExit ();
			//this.upgradesOnMouseExit ();
			settingsPanel.GetComponent<Animator> ().Play ("settings_panel_intro");
		}
	}


	//animation for hovering over mouse button
	public void reloadOnMouseOver()
	{
		if (reloadIsClicked == false) 
		{
			reloadBut.GetComponent<Animator> ().Play ("reload_highlighted");
		}
	}

	//animation for hovering out reload button(default state)
	public void reloadOnMouseExit()
	{
		if (reloadIsClicked == false)  
		{
			reloadBut.GetComponent<Animator> ().Play ("reload_normal");
		}
	}

	//animation for clicking reload button(default state)
	public void reloadOnMouseDown()
	{
		if (reloadIsClicked == false) 
		{
			reloadSettingsPanel.SetActive (true);
			reloadIsClicked = true;
			if ((inventoryIsClicked == true)) 
			{
				inventoryItems.GetComponent<Animator> ().Play ("upgrades_hideInventory");
				inventoryIsClicked = false;
				this.inventoryOnMouseExit ();
			}
			if ((settingsIsClicked == true))
			{
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked_reversed");
				settingsPanel.GetComponent<Animator> ().Play ("settings_panel_exit");
				settingsIsClicked = false;
				this.settingsOnMouseExit ();
				if (soundIsClicked == true) 
				{
					soundSettingsPanel.GetComponent<Animator> ().Play ("sound_settings_exit");
					this.soundOnMouseExit ();
					soundIsClicked = false;
				}
				if (mouseIsClicked == true) {
					mouseSettingsPanel.GetComponent<Animator> ().Play ("mouse_settings_exit");
					this.mouseOnMouseExit ();
					mouseIsClicked = false;
				}
			}
			if ((quitIsClicked == true))
			{
				quitSettingsPanel.GetComponent<Animator>().Play("quit_buttons_exit");
				quitIsClicked = false;
				this.quitOnMouseExit ();
			}
			//inventoryIsClicked = false;
			//Debug.Log (reloadIsClicked);
			//soundPanel.GetComponent<Animator> ().Play ("settings_panel_exit");
			reloadBut.GetComponent<Animator> ().Play ("reload_highlighted");
			reloadSettingsPanel.GetComponent<Animator>().Play("reload_buttons_intro");
		}
	}

	//animation for hovering over quit button
	public void quitOnMouseOver()
	{
		if (quitIsClicked == false) 
		{
			quitBut.GetComponent<Animator> ().Play ("quit_highlighted");
		}
	}


	//animation for hovering out mouse button(default state)
	public void quitOnMouseExit()
	{
		if (quitIsClicked == false)  
		{
			quitBut.GetComponent<Animator> ().Play ("quit_normal");
		}
	}

	public void quitOnMouseDown()
	{
		if (quitIsClicked == false)  
		{
			quitSettingsPanel.SetActive (true);
			if ((inventoryIsClicked == true)) 
			{
				inventoryItems.GetComponent<Animator> ().Play ("upgrades_hideInventory");
				inventoryIsClicked = false;
				this.inventoryOnMouseExit ();
			}
			if ((settingsIsClicked == true))
			{
				invSetPanel.GetComponent<Animator> ().Play ("pause_inv_set_clicked_reversed");
				settingsPanel.GetComponent<Animator> ().Play ("settings_panel_exit");
				settingsIsClicked = false;			
				this.settingsOnMouseExit ();
				if (soundIsClicked == true) 
				{
					soundSettingsPanel.GetComponent<Animator> ().Play ("sound_settings_exit");
					this.soundOnMouseExit ();
					soundIsClicked = false;
				}
				if (mouseIsClicked == true) {
					mouseSettingsPanel.GetComponent<Animator> ().Play ("mouse_settings_exit");
					this.mouseOnMouseExit ();
					mouseIsClicked = false;
				}
			}
			if ((reloadIsClicked == true))
			{
				reloadSettingsPanel.GetComponent<Animator>().Play("reload_buttons_exit");
				reloadIsClicked = false;
				this.reloadOnMouseExit ();
			}
			//inventoryIsClicked = false;
			quitIsClicked = true;
			//soundPanel.GetComponent<Animator> ().Play ("settings_panel_exit");
			quitBut.GetComponent<Animator> ().Play ("quit_highlighted");
			//inventoryItems.GetComponent<Animator> ().Play ("upgrades_hideInventory");
			//reloadSettingsPanel.GetComponent<Animator>().Play("reload_buttons_exit");
			quitSettingsPanel.GetComponent<Animator>().Play("quit_buttons_intro");
		}
	}

	/**IN DEVELOPMENT
	 * Uncomment this section for use of notes and items in inventory sub section
	 * 
	//animation for hovering over items button
	public void notesOnMouseOver()
	{
		if (itemsIsClicked == false) 
		{
			itemsBut.GetComponent<Animator> ().Play ("notes_highlighted");
		}
	}
		
	//animation for hovering out items button(default state)
	public void notesOnMouseExit()
	{
		if (itemsIsClicked == false)  
		{
			itemsBut.GetComponent<Animator> ().Play ("notes_normal");
		}
	}

	//animation for clicking notes button
	public void notesOnMouseDown()
	{
		if (itemsIsClicked == false) 
		{
			itemsBut.GetComponent<Animator> ().Play ("notes_highlighted");
			itemsIsClicked = true;
			upgradesIsClicked = false;
			this.upgradesOnMouseExit ();
			inventoryItems.SetActive (true);
			inventoryItems.GetComponent<Animator> ().Play ("inventory_show");
		}
	}

	//animation for hovering over upgrades button
	public void upgradesOnMouseOver()
	{
		if (upgradesIsClicked == false) 
		{
			upgradesBut.GetComponent<Animator> ().Play ("upgrades_highlighted");
		}
	}

	//animation for hovering out upgrades button(default state)
	public void upgradesOnMouseExit()
	{
		if (upgradesIsClicked == false) 
		{
			upgradesBut.GetComponent<Animator> ().Play ("upgrades_normal");
		}
	}

	//animation for hovering over upgrades button
	public void upgradesOnMouseDown()
	{
		if (upgradesIsClicked == false) 
		{
			upgradesBut.GetComponent<Animator> ().Play ("upgrades_highlighted");
			itemsIsClicked = false;
			upgradesIsClicked = true;
			this.notesOnMouseExit ();
			inventoryItems.GetComponent<Animator> ().Play ("upgrades_hideInventory");
		}
	}
	**/
	//animation for hovering over sound button
	public void soundOnMouseOver()
	{
		if (soundIsClicked == false) 
		{
			soundBut.GetComponent<Animator> ().Play ("sound_highlighted");
		}
	}

	//animation for hovering out sound button(default state)
	public void soundOnMouseExit()
	{
		if (soundIsClicked == false)  
		{
			soundBut.GetComponent<Animator> ().Play ("sound_normal");
		}
	}

	//animation for clicking sound button
	//clicking it enables the sound settings
	public void soundOnMouseDown()
	{
		if (soundIsClicked == false) 
		{
			soundBut.GetComponent<Animator> ().Play ("sound_highlighted");
			mouseIsClicked = false;
			soundIsClicked = true;
			this.mouseOnMouseExit ();
			soundSettingsPanel.SetActive (true);
			soundSettingsPanel.GetComponent<Animator> ().Play ("sound_settings_intro");
			mouseSettingsPanel.GetComponent<Animator> ().Play ("mouse_settings_exit");
			mouseSettingsPanel.SetActive (false);
		}
	}

	//animation for hovering over mouse button
	public void mouseOnMouseOver()
	{
		if (mouseIsClicked == false) 
		{
			mouseBut.GetComponent<Animator> ().Play ("mouse_highlighted");
		}
	}

	//animation for hovering out mouse button(default state)
	public void mouseOnMouseExit()
	{
		if (mouseIsClicked == false)  
		{
			mouseBut.GetComponent<Animator> ().Play ("mouse_normal");
		}
	}

	//animation for clicking mouse button
	//clicking enables the mouse settings(x and y sensitivity)
	public void mouseOnMouseDown()
	{
		if (mouseIsClicked == false) 
		{
			mouseBut.GetComponent<Animator> ().Play ("mouse_highlighted");
			mouseIsClicked = true;
			soundIsClicked = false;
			this.soundOnMouseExit ();
			mouseSettingsPanel.SetActive (true);
			soundSettingsPanel.GetComponent<Animator> ().Play ("sound_settings_exit");
			mouseSettingsPanel.GetComponent<Animator> ().Play ("mouse_settings_intro");
		}
	}

	//animation for yes button  when asking reload
	public void yesReloadOnMouseOver()
	{
		if (reloadIsClicked == true) 
		{
			yesReloadBut.GetComponent<Animator> ().Play ("yes_reload_highlighted");
		}
	}

	//animation for yes button when exiting 
	public void yesReloadOnMouseExit()
	{
		if (reloadIsClicked == true) 
		{
			yesReloadBut.GetComponent<Animator> ().Play ("yes_reload_normal");
		}	
	}

	//animation for clicking yes when asking reload
	public void yesReloadOnMousedDown()
	{
		if (reloadIsClicked == true) 
		{
			yesReloadBut.GetComponent<Animator> ().Play ("yes_reload_highlighted");
			this.ReturnToCheckpoint ();
		}	
	}

	//reload level
	public void ReturnToCheckpoint()
	{

		PauseGame();
		EventManager.PostNotification((int) GameManagerScript.GameEvents.LoadLastCheckpointPressed, this, null);


		Debug.Log ("reloaded");
	}

	//animation for yes button when asking quit
	public void yesQuitOnMouseOver()
	{
		if (quitIsClicked == true) 
		{
			yesQuitBut.GetComponent<Animator>().Play("yes_quit_highlighted");
		}
	}

	//animation for exitting yes button when asking quit
	public void yesQuitOnMouseExit()
	{
		if (quitIsClicked == true) 
		{
			yesQuitBut.GetComponent<Animator>().Play("yes_quit_normal");
		}
	}

	//animation for clicking yes when asking quit
	//quits game
	public void yesQuitOnMouseDown()
	{
		if (quitIsClicked == true) 
		{
			yesQuitBut.GetComponent<Animator>().Play("yes_quit_highlighted");
			Application.Quit();
		}
	}
		
	// Update is called once per frame
	//starts pause game when player presses p or escape button
	void Update () {
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame ();	
		}
	}
}
