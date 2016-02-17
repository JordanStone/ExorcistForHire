using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Bullet_Handler : MonoBehaviour {
	
	public float spawnTime;
	public float destroyTime;
	
	
	private float destroyTimer;
	private bool dying = false;
	
	
	// Use this for initialization
	void Start () {
		DOTween.Init();
		
		
		transform.DOScale(0.005949641f, spawnTime);
	}
	
	// Update is called once per frame
	void Update () {
		if (dying == true)
		{
			
			destroyTimer -= Time.deltaTime;
			if (destroyTimer <= 0f)
			{
				Destroy(gameObject);
			}
		}
		
	}
	
	
	void DestroyMe()
	{
		transform.SetParent(transform.parent.parent);
		transform.DOScale(0f, destroyTime);
		transform.DOLocalMoveZ( .009f, destroyTime);
		dying = true;
		destroyTimer = destroyTime;
	}
}
