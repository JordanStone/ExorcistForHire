using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Subtitles : MonoBehaviour {
	public string[] Lines;
	public GameObject subtitles;
	public GameObject manager;
	private Text myText;
	private int gate;
	private bool inRange;
	void Start() {
		subtitles = GameObject.Find("Subtitles");
		manager = GameObject.Find ("GameManager");
		myText = subtitles.GetComponent<Text>();
		inRange = false;
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
			if(gate >= Lines.Length) {
				gate = 0;
			}
			myText.text = Lines[gate];
		}
	}
}
