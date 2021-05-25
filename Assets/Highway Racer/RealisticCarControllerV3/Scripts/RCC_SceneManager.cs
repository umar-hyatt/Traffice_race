//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene manager that contains current player vehicle, current player camera, current player UI, current player character, recording/playing mechanim, and other vehicles as well.
/// 
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Scene Manager")]
public class RCC_SceneManager : MonoBehaviour {

	#region singleton
	private static RCC_SceneManager instance;
	public static RCC_SceneManager Instance{
		
		get{
			
			if (instance == null) {

				instance = FindObjectOfType<RCC_SceneManager> ();

				if (instance == null) {
					
					GameObject sceneManager = new GameObject ("_RCCSceneManager");
					instance = sceneManager.AddComponent<RCC_SceneManager> ();

				}

			}
			
			return instance;

		}

	}

	#endregion

	public RCC_CarControllerV3 activePlayerVehicle;
	public RCC_UIDashboardDisplay activePlayerCanvas;
	public Camera activeMainCamera;

	public bool registerFirstVehicleAsPlayer = true;
	public bool disableUIWhenNoPlayerVehicle = false;

	public enum RecordMode{Neutral, Play, Record}
	public RecordMode recordMode;

	// Default time scale of the game.
	private float orgTimeScale = 1f;

	public List <RCC_CarControllerV3> allVehicles = new List<RCC_CarControllerV3> ();

	void Awake(){

		RCC_CarControllerV3.OnRCCPlayerSpawned += RCC_CarControllerV3_OnRCCSpawned;
		RCC_CarControllerV3.OnRCCPlayerDestroyed += RCC_CarControllerV3_OnRCCPlayerDestroyed;
		activePlayerCanvas = GameObject.FindObjectOfType<RCC_UIDashboardDisplay> ();

		// Getting default time scale of the game.
		orgTimeScale = Time.timeScale;
		
	}

	#region ONSPAWNED

	void RCC_CarControllerV3_OnRCCSpawned (RCC_CarControllerV3 RCC){

		if (!allVehicles.Contains (RCC))
			allVehicles.Add (RCC);

		if (registerFirstVehicleAsPlayer)
			SetPlayer (RCC);

	}

	#endregion

	#region ONDESTROYED

	void RCC_CarControllerV3_OnRCCPlayerDestroyed (RCC_CarControllerV3 RCC){

		if (allVehicles.Contains (RCC))
			allVehicles.Remove (RCC);

	}

	#endregion

	void Update(){

		if(disableUIWhenNoPlayerVehicle && activePlayerCanvas)
			CheckCanvas ();

		if (Input.GetKey (RCC_Settings.Instance.slowMotionKB))
			Time.timeScale = .2f;

		if (Input.GetKeyUp (RCC_Settings.Instance.slowMotionKB))
			Time.timeScale = orgTimeScale;

		activeMainCamera = Camera.current;

	}

	public void SetPlayer(RCC_CarControllerV3 playerVehicle){

		activePlayerVehicle = playerVehicle;

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void SetPlayer(RCC_CarControllerV3 playerVehicle, bool isControllable){

		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void SetPlayer(RCC_CarControllerV3 playerVehicle, bool isControllable, bool engineState){

		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);
		activePlayerVehicle.SetEngine (engineState);

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void DeRegisterPlayer(){

		if (activePlayerVehicle)
			activePlayerVehicle.SetCanControl (false);
		
		activePlayerVehicle = null;

	}

	public void CheckCanvas(){

		if (!activePlayerVehicle || !activePlayerVehicle.gameObject.activeInHierarchy || !activePlayerVehicle.enabled) {

//			if (activePlayerCanvas.displayType == RCC_UIDashboardDisplay.DisplayType.Full)
//				activePlayerCanvas.SetDisplayType(RCC_UIDashboardDisplay.DisplayType.Off);

			activePlayerCanvas.SetDisplayType(RCC_UIDashboardDisplay.DisplayType.Off);

			return;

		}

//		if(!activePlayerCanvas.gameObject.activeInHierarchy)
//			activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;

		if(activePlayerCanvas.displayType != RCC_UIDashboardDisplay.DisplayType.Customization)
			activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;

	}

	void OnDisable(){

		RCC_CarControllerV3.OnRCCPlayerSpawned -= RCC_CarControllerV3_OnRCCSpawned;
		RCC_CarControllerV3.OnRCCPlayerDestroyed -= RCC_CarControllerV3_OnRCCPlayerDestroyed;

	}

}
