//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[System.Serializable]
public class HR_HighwayRacerProperties : ScriptableObject {

	public static HR_HighwayRacerProperties instance;
	public static HR_HighwayRacerProperties Instance
	{
		get
		{
			if(instance == null)
				instance = Resources.Load("HR_Assets/HR_HighwayRacerProperties") as HR_HighwayRacerProperties;
			return instance;
		}

	}

	public bool usePostProcessingImageEffects = true;

	public int _minimumSpeedForGainScore;
	public int _minimumSpeedForHighSpeed;
	public int _minimumCollisionForGameOver;

	public Color _defaultBodyColor;
	public bool _defaultBloom;
	public bool _defaultAO;
	public bool _defaultBlur;
	public bool _defaultHQLights;
	public bool _shakeCamera;
	public bool _tiltCamera;
	public int _totalDistanceMoneyMP;
	public int _totalNearMissMoneyMP;
	public int _totalOverspeedMoneyMP;
	public int _totalOppositeDirectionMP;

	public int toolbarSelectedIndex;
	public bool _1MMoneyForTesting;

	public GameObject[] selectablePlayerCars;
	public GameObject[] upgradableWheels;
	public GameObject attachableSiren;
	public GameObject explosionEffect;

	public GameObject exhaustGas;

	public AudioClip[] mainMenuClips;
	public AudioClip[] gameplayClips;
	public AudioClip buttonClickAudioClip;
	public AudioClip nearMissAudioClip;
	public AudioClip labelSlideAudioClip;
	public AudioClip countingPointsAudioClip;
	public AudioClip bombTimerAudioClip;
	public AudioClip sirenAudioClip;

	public LayerMask trafficCarsLayer;

}
