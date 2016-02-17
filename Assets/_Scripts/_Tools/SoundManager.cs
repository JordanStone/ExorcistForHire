using UnityEngine;
using System.Collections;
using System.Linq;
using NashTools;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Audio;


namespace SoundController
{
	// Manager created to better organize and abstract management of audio sources. 
	public class SoundManager : MonoBehaviour {
		
		public AudioClip[] ambience; // Ambience Tracks
		public AudioClip[] walks; // Walk Noises
		public AudioClip[] gunshots; // Gun Noises
		public AudioClip[] sfx;
//		public AudioSource[] ghostnoises; // Ghost Noises
		private AudioSource[] audioSources; // All Sounds

		public float ambienceFadeTime = 1f;


		// Current Ambience track. There should only ever be one playing.
		public AudioSource ambienceSound;


		[Header("This Plays if a sound is null in the code.")]
		public AudioClip ErrorSound;

		// Use this for initialization
		void Awake () 
		{
			audioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[]; 

			// Keep an array that tracks all sounds for convenience
//			audioSources = ambience.Concat(walks).Concat(gunshots).Concat(ghostnoises).ToArray();
//			audioSources = ambience.Concat(walks).Concat(gunshots).ToArray();
			
			// Set Ambience to first track
			SetAmbience( NullTest(ambience, 0));
		}

		void Start()
		{
			DOTween.Init();
			EventManager.AddListener((int) GameManagerScript.GameEvents.LoadlastCheckpoint, OnLoadLastCheckpoint);
		}


	//No More Singleton sound manager.
	/*	
		private static SoundManager s_Instance = null;
		
		// This defines a static instance property that attempts to find the manager object in the scene and
		// returns it to the caller.
		public static SoundManager instance {
			get {
				if (s_Instance == null) {
					// This is where the magic happens.
					//  FindObjectOfType(...) returns the first AManager object in the scene.
					s_Instance =  FindObjectOfType(typeof (SoundManager)) as SoundManager;
				}
				
				// If it is still null, create a new instance
				if (s_Instance == null) {
					GameObject obj = new GameObject("SoundManager");
					s_Instance = obj.AddComponent(typeof (SoundManager)) as SoundManager;
					Debug.Log ("Could not locate an AManager object. SoundManager was Generated Automaticly.");
				}
				
				return s_Instance;
			}
		}
		
		// Ensure that the instance is destroyed when the game is stopped in the editor.
		void OnApplicationQuit() {
			s_Instance = null;
		}
		*/

	// ---Private Functions---

		//if the audio source is null then return the error sound
		private AudioClip NullTest( AudioClip TestThis)
		{
			if (TestThis == null)
				return ErrorSound;
			else
				return TestThis;
		}

		//Null test for arrays
		private AudioClip NullTest(AudioClip[] TestArray, int i)
		{
			if( (i + 1) <= TestArray.Count())
			{
				if (TestArray[i])
					return TestArray[i];
				else
					return ErrorSound;
			}
			else
			{
				Debug.Log("ERROR: No Audiosource at: " + TestArray.ToString() +" "+ i.ToString());
				return ErrorSound;
			}
		}

		// On restarts, reactivate all sounds that play on awake and set ambience to start playing again
		private void OnLoadLastCheckpoint(Component poster, object checkpointData)
		{
			ambienceSound.Play();
			foreach(AudioSource source in audioSources)
			{
				if (source && source.playOnAwake)
				{
					source.Play();
				}
			}
		}

		
	// ---Public Functions---

		// Pause all sound
		public void StopAllAudio()
		{
			foreach(AudioSource source in audioSources)
			{
				if(source)
					source.Stop();
			}
		}
		
		// Continue playing of sound after pause
		public void ContinueAllAudio()
		{
			foreach(AudioSource source in audioSources)
			{
				if(source)
					source.UnPause();
			}
		}

		// Pause all except for ambience, useful for pause screens.
		public void PauseAllButAmbience()
		{
			foreach(AudioSource source in audioSources)
			{
				if (source)
					source.Pause();
			}
			ambienceSound.UnPause();
		}
		
		// Set an ambience track to play, turn off the one currently playing (if there is one)
		// Planning on adding fade-in soon because that would be fancy as hell
		public void SetAmbience(AudioClip amb)
		{
			//Test if amb is null first.
			NullTest(amb);

			if (ambienceSound != null && ambienceSound.clip != amb)
			{
				if (ambienceSound.isPlaying)
				{
	//				print("Let's switch it up!");
					ambienceSound.DOFade(0f, ambienceFadeTime);
					StartCoroutine(WaitTime(ambienceFadeTime, amb));
				}
				else
				{
					ambienceSound.clip = amb;
					ambienceSound.Play();
				}
			}
		}

		IEnumerator WaitTime(float time, AudioClip amb) 
		{
			yield return new WaitForSeconds(time);
			ambienceSound.Stop();
			ambienceSound.clip = amb;
//			float vol = ambienceSound.volume;

			ambienceSound.volume = 0f;
			ambienceSound.Play();
			ambienceSound.DOFade(1f, time);
		}
		
	// Getters		
		public AudioClip GetWalkSound(int index)
		{
			return NullTest(walks, index);
		}
		
		public AudioClip GetAmbientSound(int index)
		{
			return NullTest(ambience, index);
		}
		
		public AudioClip GetGunSound(int index)
		{
			return NullTest(gunshots, index);
		}

		public AudioClip GetSFXSound(int index)
		{
			return NullTest(sfx, index);
		}

		// Returns current ambience track playing
		public AudioClip GetAmbienceSound()
		{
			return NullTest(ambienceSound.clip);
		}

		public void ChangeVolume(){
			float value_ = GameObject.Find ("Audio Slinder").GetComponent <Slider> ().value;

			foreach(AudioSource source in audioSources)
			{
				if(source)
					source.volume=value_;
			}
		}
	
	}
}

