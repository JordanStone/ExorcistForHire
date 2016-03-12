using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LoadingScreenNamespace
{
	public class LoadingScreen : MonoBehaviour
	{
		static LoadingScreen instance;

		private Image _loadingImage;
		public Image LoadingSlider;
		
		private AsyncOperation async = null;


		void Start()
		{
			if (instance)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;    
			DontDestroyOnLoad(this); 
		
			_loadingImage = GetComponent<Image>();
			_loadingImage.enabled = false;
		}


		void Update()
		{
			if(async != null)
			{
				LoadingSlider.fillAmount = async.progress;
			}
		}

		public void LoadLevel(string sceneName)
		{
			_loadingImage.enabled = true;
		
			StartCoroutine("Loading", sceneName);
		}
	

		public IEnumerator Loading(string sceneName)
		{
			async = Application.LoadLevelAsync(sceneName);

			yield return async;
			
			Debug.Log ("loading Complete");			
		}


		static bool InstanceExists()
		{
			if (!instance)
			{
				return false;
			}
			return true;
			
		}
				
	}
}