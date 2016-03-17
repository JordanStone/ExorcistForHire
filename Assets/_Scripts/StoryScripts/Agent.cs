using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Agent : MonoBehaviour {
public string[] InitialLines;
	public string[] HasTalkedLines;
	public GameObject subtitles;
	public GameObject manager;
	public bool hasTalked;
	public GameObject[] Enemies;
	public Material sky;
	private Text myText;
	public int gate;
	private bool inRange;
	public bool changeSky;
	void Start() {
		subtitles = GameObject.Find("Subtitles");
		manager = GameObject.Find ("GameManager");
		myText = subtitles.GetComponent<Text>();
		inRange = false;
		hasTalked = false;
	}
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "Player") {
			myText.text = "Examine";
			inRange = true;
		}
	}
	void OnTriggerExit () {
		myText.text = "";
		inRange = false;
	}
	public void DisplayLine() {
		if(inRange == true) {
			if(hasTalked == false) {
				myText.text = InitialLines[0];
				hasTalked = true;
				manager.GetComponent<GameManagerScript>().setGate(gate);
				foreach(GameObject g in Enemies) {
					g.SetActive(true);
				}
			} else {
				myText.text = HasTalkedLines[0];
			}
		}
		if(changeSky == true) {
			RenderSettings.skybox = sky;
		}
	}
}