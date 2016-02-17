using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Scares
{
	public class FoldingWall_Scare : MonoBehaviour 
	{
		[Range(0.0f, 100.0f)]
		public float FoldingWallEndBlend;
		public float BlendSeconds;

		private float _blendValue;
		public Material FoldingWallMaterial;
		public Collider SpookTrigger;

		private bool hasSpooked;
		private AudioSource spookNoise;

		// Use this for initialization
		void Start () 
		{
			//Reset blend to 0 on start
			FoldingWallMaterial.SetFloat ("_Opacity", 0f);

			spookNoise = GetComponent<AudioSource>();
		}
		
		// Update is called once per frame
		void Update () 
		{
	
		}

		void OnTriggerEnter(Collider other) 
		{
			//If the player entered the trigger
			if (other.tag == "Player" && !hasSpooked) 
			{
				//Trigger the spook.
				FoldingWallBlendChange( FoldingWallEndBlend, BlendSeconds);
				spookNoise.Play();
				hasSpooked = true;
			}
		} 


		public void FoldingWallBlendChange( float EndValue, float Seconds)
		{
			DOTween.To(()=> FoldingWallMaterial.GetFloat("_Opacity"), x=> FoldingWallMaterial.SetFloat("_Opacity", x), EndValue, Seconds);

		}

	}

}
