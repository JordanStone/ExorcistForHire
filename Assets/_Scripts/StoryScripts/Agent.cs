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
				myText.text = InitialLines[gate];
				hasTalked = true;
				manager.GetComponent<GameManagerScript>().setGate(2);
				foreach(GameObject g in Enemies) {
					g.SetActive(true);
				}
			} else {
				myText.text = HasTalkedLines[gate];
			}
		}
	}
}