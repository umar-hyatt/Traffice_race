//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class HR_GamePlayHandler : MonoBehaviour {

	#region SINGLETON PATTERN
	public static HR_GamePlayHandler _instance;
	public static HR_GamePlayHandler Instance
	{
		get
		{
			if (_instance == null){
				_instance = GameObject.FindObjectOfType<HR_GamePlayHandler>();
			}
			
			return _instance;
		}
	}
	#endregion

	[Header("Time Of The Scene")]
	public DayOrNight dayOrNight;
	public enum DayOrNight{Day, Night, Rainy, Snowy}

	[Header("Current Mode")]
	internal Mode mode;
	internal enum Mode {OneWay, TwoWay, TimeAttack, Bomb}

	[Header("UI Canvases For GamePlay and GameOver")]
	public Canvas gameplayCanvas;
	public Canvas gameoverCanvas;

	[Header("Spawn Location Of The Cars")]
	public Transform spawnLocation;

	[HideInInspector]public GameObject player;

	private int selectedCarIndex = 0;
	private int selectedModeIndex = 0;
	private float minimumSpeed = 20f;

	internal bool gameStarted = false;
	internal bool paused = false;

	public int totalPlayedCount = 0;

	public delegate void onPaused();
	public static event onPaused OnPaused;

	public delegate void onResumed();
	public static event onResumed OnResumed;

	private AudioSource gameplaySoundtrack;

	void Awake(){

		Time.timeScale = 1f;
		AudioListener.volume = 0f;
		AudioListener.pause = false;

		if (HR_HighwayRacerProperties.Instance.gameplayClips != null && HR_HighwayRacerProperties.Instance.gameplayClips.Length > 0) {
			gameplaySoundtrack = RCC_CreateAudioSource.NewAudioSource (gameObject, "GamePlay Soundtrack", 0f, 0f, .35f, HR_HighwayRacerProperties.Instance.gameplayClips [UnityEngine.Random.Range (0, HR_HighwayRacerProperties.Instance.gameplayClips.Length)], true, true, false);
			gameplaySoundtrack.ignoreListenerPause = true;
			gameplaySoundtrack.ignoreListenerVolume = true;
		}

		selectedCarIndex = PlayerPrefs.GetInt("SelectedPlayerCarIndex");
		selectedModeIndex = PlayerPrefs.GetInt("SelectedModeIndex");
		totalPlayedCount = PlayerPrefs.GetInt("TotalPlayedCount");

		switch(selectedModeIndex){

		case 0:
			mode = Mode.OneWay;
			break;
		case 1:
			mode = Mode.TwoWay;
			break;
		case 2:
			mode = Mode.TimeAttack;
			break;
		case 3:
			mode = Mode.Bomb;
			break;

		}

	}

	void Start () {

		gameplayCanvas.enabled = true;

		SpawnCar();
		StartCoroutine(WaitForGameStart());
	
	}

	void OnEnable(){

		SceneManager.sceneLoaded += SceneManager_sceneLoaded;

		HR_PlayerHandler.OnPlayerSpawned += HR_PlayerHandler_OnPlayerSpawned;
		HR_PlayerHandler.OnNearMiss += HR_PlayerHandler_OnNearMiss;
		HR_PlayerHandler.OnPlayerDied += HR_PlayerHandler_OnPlayerDied;

	}

	void HR_PlayerHandler_OnPlayerSpawned (HR_PlayerHandler player){

		gameStarted = false;
		RCC.SetControl (player.GetComponent<RCC_CarControllerV3>(), false);
		StartCoroutine (WaitForGameStart ());
		
	}

	void HR_PlayerHandler_OnNearMiss (HR_PlayerHandler player, int score, HR_DynamicScoreDisplayer.Side side){
		


	}

	void HR_PlayerHandler_OnPlayerDied (HR_PlayerHandler player){

		StartCoroutine (OnGameOver (1f));
		
	}

	void SceneManager_sceneLoaded (Scene arg0, LoadSceneMode arg1){

		Time.timeScale = 1;
		AudioListener.pause = false;
		
	}

	IEnumerator WaitForGameStart(){

		yield return new WaitForSeconds(4);

		RCC.SetControl (player.GetComponent<RCC_CarControllerV3> (), true);
		gameStarted = true;

	}

	void Update(){

		if(AudioListener.volume < 1 && !paused && Time.timeSinceLevelLoad > .5f){
			
			AudioListener.volume = Mathf.MoveTowards(AudioListener.volume, 1f, Time.deltaTime);
			
		}

	}

	void SpawnCar () {

		if(mode != Mode.Bomb)
			player = (RCC.SpawnRCC(HR_PlayerCars.Instance.cars[selectedCarIndex].playerCar.GetComponent<RCC_CarControllerV3>(), spawnLocation.position, spawnLocation.rotation, true, false, true)).gameObject;
		else
			player = (RCC.SpawnRCC(HR_PlayerCars.Instance.bombedVehicleForBombMode.GetComponent<RCC_CarControllerV3>(), spawnLocation.position, spawnLocation.rotation, true, false, true)).gameObject;

		player.transform.position = spawnLocation.transform.position;
		player.transform.rotation = Quaternion.identity;

		player.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, minimumSpeed / 1.75f);

		if(dayOrNight == DayOrNight.Night || dayOrNight == DayOrNight.Rainy)
			player.GetComponent<RCC_CarControllerV3>().lowBeamHeadLightsOn = true;
	
	}

	public IEnumerator OnGameOver(float delayTime){
		
		gameplayCanvas.enabled = false;
		yield return new WaitForSecondsRealtime(delayTime);
		OnPaused ();

		switch(mode){

		case Mode.OneWay:
			PlayerPrefs.SetInt("bestScoreOneWay", (int)player.GetComponent<HR_PlayerHandler>().score);
			break;
		case Mode.TwoWay:
			PlayerPrefs.SetInt("bestScoreTwoWay", (int)player.GetComponent<HR_PlayerHandler>().score);
			break;
		case Mode.TimeAttack:
			PlayerPrefs.SetInt("bestScoreTimeAttack", (int)player.GetComponent<HR_PlayerHandler>().score);
			break;
		case Mode.Bomb:
			PlayerPrefs.SetInt("bestScoreBomb", (int)player.GetComponent<HR_PlayerHandler>().score);
			break;

		}

		totalPlayedCount++;
		PlayerPrefs.SetInt("TotalPlayedCount", totalPlayedCount);

	}

	public void MainMenu(){

		SceneManager.LoadScene(0);

	}

	public void RestartGame(){
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
	}

	public void Paused(){

		paused = !paused;

		if(paused)
			OnPaused ();
		else
			OnResumed ();

	}

	void OnDisable(){

		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

		HR_PlayerHandler.OnPlayerSpawned -= HR_PlayerHandler_OnPlayerSpawned;
		HR_PlayerHandler.OnNearMiss -= HR_PlayerHandler_OnNearMiss;
		HR_PlayerHandler.OnPlayerDied -= HR_PlayerHandler_OnPlayerDied;

	}

}
