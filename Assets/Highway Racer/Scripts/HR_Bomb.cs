//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_Bomb : MonoBehaviour {

	private HR_PlayerHandler handler;
	private Light bombLight;

	private float signalTimer = 0f;

	private AudioSource bombTimerAudioSource;
	private AudioClip bombTimerAudioClip{get{return HR_HighwayRacerProperties.Instance.bombTimerAudioClip;}}

	void Start () {

		handler = GetComponentInParent<HR_PlayerHandler>();
		bombTimerAudioSource = RCC_CreateAudioSource.NewAudioSource(gameObject, "Bomb Timer AudioSource", 0f, 0f, .25f, bombTimerAudioClip, false, false, false);
		bombLight = GetComponentInChildren<Light>();
		bombLight.enabled = true;
		bombLight.intensity = 0f;
	
	}

	void FixedUpdate () {

		if(!handler)
			return;

		if(!handler.bombTriggered)
			return;

		signalTimer += Time.fixedDeltaTime * Mathf.Lerp(5f, 1f, handler.bombHealth / 100f);

		if(signalTimer >= .5f){
			bombLight.intensity = Mathf.Lerp(bombLight.intensity, 0f, Time.fixedDeltaTime * 50f);
		}else{
			bombLight.intensity = Mathf.Lerp(bombLight.intensity, 1f, Time.fixedDeltaTime * 50f);
		}

		if(signalTimer >= 1f){
			signalTimer = 0f;
			bombTimerAudioSource.Play();
		}
	
	}

}
