//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HR_DynamicScoreDisplayer : MonoBehaviour {

	#region SINGLETON PATTERN
	public static HR_DynamicScoreDisplayer _instance;
	public static HR_DynamicScoreDisplayer Instance
	{
		get
		{
			if (_instance == null){
				_instance = GameObject.FindObjectOfType<HR_DynamicScoreDisplayer>();
			}

			return _instance;
		}
	}
	#endregion

	private Text scoreText;
	private Text[] scoreTexts;
	private int index = 0;

	private float lifeTime = 1f;
	private float timer = 0f;
	private Vector3 defPos;

	public enum Side{Left, Right, Center}

	private AudioSource nearMissSound;

	void Start () {

		scoreText = GetComponentInChildren<Text> ();
		scoreText.gameObject.SetActive (false);

		scoreTexts = new Text[10];

		for (int i = 0; i < 10; i++) {

			GameObject instantiatedText = GameObject.Instantiate (scoreText.gameObject, transform);
			scoreTexts[i] = instantiatedText.GetComponent<Text>();
			scoreTexts [i].color = new Color (scoreTexts [i].color.r, scoreTexts [i].color.g, scoreTexts [i].color.b, 0f);
			scoreTexts[i].gameObject.SetActive (true);

		}

		timer = 0f;
		defPos = scoreTexts[0].transform.position;

	}

	void OnEnable(){

		HR_PlayerHandler.OnNearMiss += HR_PlayerHandler_OnNearMiss;

	}

	void HR_PlayerHandler_OnNearMiss (HR_PlayerHandler player, int score, Side side){

		switch (side) {

		case Side.Left:
			DisplayScore (score, -75f);
			break;

		case Side.Right:
			DisplayScore (score, 75f);
			break;

		case Side.Center:
			DisplayScore (score, 0f);
			break;

		}
		
	}

	public void DisplayScore (int score, float offset) {

		if (index < scoreTexts.Length - 1)
			index++;
		else
			index = 0;

		scoreTexts[index].text = "+" + score.ToString ();
		scoreTexts[index].transform.position = new Vector3 (defPos.x + offset, defPos.y, defPos.z);

		timer = lifeTime;
		nearMissSound = RCC_CreateAudioSource.NewAudioSource (gameObject, HR_HighwayRacerProperties.Instance.nearMissAudioClip.name, 0f, 0f, 1f, HR_HighwayRacerProperties.Instance.nearMissAudioClip, false, true, true);
		nearMissSound.ignoreListenerPause = true;
		nearMissSound.ignoreListenerVolume = true;

	}

	void Update(){
		
		if(timer > 0)
			timer -= Time.deltaTime;

		timer = Mathf.Clamp (timer, 0f, lifeTime);

		for (int i = 0; i < scoreTexts.Length; i++) {
//			scoreTexts [i].transform.Translate (Vector3.up * Time.deltaTime * 75f, Space.World);
			scoreTexts[i].color = Color.Lerp (scoreTexts[i].color, new Color(scoreTexts[i].color.r, scoreTexts[i].color.g, scoreTexts[i].color.b, 0f), Time.deltaTime * 10f);
		}

		if (timer > 0) {

			scoreTexts[index].color = new Color(scoreTexts[index].color.r, scoreTexts[index].color.g, scoreTexts[index].color.b, 1f);

		}

	}

	void OnDisable(){

		HR_PlayerHandler.OnNearMiss -= HR_PlayerHandler_OnNearMiss;

	}

}
