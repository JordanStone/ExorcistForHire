using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Subtitles : MonoBehaviour {
	public string[] InitialLines;
	public string[] HasTalkedLines;
	public GameObject subtitles;
	public GameObject manager;
	public bool hasTalked;
	private Text myText;
	private int gate;
	private bool inRange;
	void Start() {
		subtitles = GameObject.Find("Subtitles");
		manager = GameObject.Find ("GameManager");
		myText = subtitles.GetComponent<Text>();
		inRange = false;
		hasTalked = false;
	}
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name == "Player") {
			myText.text = "Click to Talk!";
			inRange = true;
		}
	}
	void OnTriggerExit () {
		myText.text = "";
		inRange = false;
	}
	public void DisplayLine() {
		if(inRange == true) {
			gate = manager.GetComponent<GameManagerScript>().getGate();
			if(gate >= InitialLines.Length) {
				gate = 0;
			}
			if(hasTalked == false) {
				myText.text = InitialLines[gate];
				Debug.Log("Initial");
				hasTalked = true;
			} else {
				myText.text = HasTalkedLines[gate];
				Debug.Log("HasTalked");
			}
		}
	}
}
