//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

//Modification Script For Checking/Applying User Modifications. Also Controls UI Buttons, Sliders and Texts About Modding

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HR_ModHandler : MonoBehaviour {

	//Classes
	private HR_MainMenuHandler menuHandler;
	private RCC_CarControllerV3 currentCar;
	private HR_ModApplier currentApplier;

	//UI Panels
	[Header("Modify Panels")]
	public GameObject colorClass;
	public GameObject wheelClass;
	public GameObject upgradesClass;

	//UI Buttons
	[Header("Modify Buttons")]
	public Button bodyPaintButton;
	public Button rimButton;
	public Button upgradeButton;
	private Color orgButtonColor;

	//UI Sliders
	[Header("Power Bars")]
	public Slider speedBar;
	public Slider handlingBar;
	public Slider brakeBar;

	[Header("Maximum Power Bars")]
	public Slider maxSpeedBar;
	public Slider maxHandlingBar;
	public Slider maxBrakeBar;

	//UI Texts
	[Header("Upgrade Levels Texts")]
	public Text speedUpgradeLevel;
	public Text handlingUpgradeLevel;
	public Text brakeUpgradeLevel;
	public Text sirenUpgradeLevel;
	public Text nosUpgradeLevel;
	public Text turboUpgradeLevel;

	void Awake () {

		orgButtonColor = bodyPaintButton.image.color;
		menuHandler = GameObject.FindObjectOfType<HR_MainMenuHandler>();

	}

	void Update(){

		currentCar = menuHandler.currentCarControllers[menuHandler.carIndex];
		currentApplier = menuHandler.currentModAppliers[menuHandler.carIndex];

		if (!currentCar || !currentApplier)
			return;

		if(speedBar)
			speedBar.value = Mathf.Lerp(speedBar.value, currentCar.maxspeed / 350f, Time.deltaTime * 5f);
		if(handlingBar)
			handlingBar.value = Mathf.Lerp(handlingBar.value, currentCar.highwaySteeringHelper / 10f, Time.deltaTime * 5f);
		if(brakeBar)
			brakeBar.value = Mathf.Lerp(brakeBar.value, currentCar.highwayBrakingHelper / 10f, Time.deltaTime * 5f);

		if(maxSpeedBar)
			maxSpeedBar.value = Mathf.Lerp(maxSpeedBar.value, currentApplier.maxUpgradeSpeed / 350f, Time.deltaTime * 5f);
		if(maxHandlingBar)
			maxHandlingBar.value = Mathf.Lerp(maxHandlingBar.value, currentApplier.maxUpgradeHandling / 10f, Time.deltaTime * 5f);
		if(maxBrakeBar)
			maxBrakeBar.value = Mathf.Lerp(maxBrakeBar.value, currentApplier.maxUpgradeBrake / 10f, Time.deltaTime * 5f);

		if(speedUpgradeLevel)
			speedUpgradeLevel.text = currentApplier.speedLevel.ToString("F0");
		if(handlingUpgradeLevel)
			handlingUpgradeLevel.text = currentApplier.handlingLevel.ToString("F0");
		if(brakeUpgradeLevel)
			brakeUpgradeLevel.text = currentApplier.brakeLevel.ToString("F0");
		if(sirenUpgradeLevel)
			sirenUpgradeLevel.text = currentApplier.isSirenPurchased && currentApplier.attachedFrontSiren.activeSelf ? "ON" : "OFF";
		if(nosUpgradeLevel)
			nosUpgradeLevel.text = currentApplier.isNOSPurchased ? "ON" : "OFF";
		if(turboUpgradeLevel)
			turboUpgradeLevel.text = currentApplier.isTurboPurchased ? "ON" : "OFF";

	}

	public void ChooseClass(GameObject activeClass){

		colorClass.SetActive(false);
		wheelClass.SetActive(false);
		upgradesClass.SetActive(false);

		activeClass.SetActive(true);

	}

	public void CheckButtonColors(Button activeButton){

		bodyPaintButton.image.color = orgButtonColor;
		rimButton.image.color = orgButtonColor;
		upgradeButton.image.color = orgButtonColor;

		activeButton.image.color = new Color(.65f, 1f, 0f);

	}

	public void ChangeChassisColor (Color color) {

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.bodyColor = color;
		applier.UpdateStats();

	}

	public void ChangeWheels (int wheelIndex) {

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.selectedWheel = HR_Wheels.Instance.wheels[wheelIndex].wheel;
		applier.wheelIndex = wheelIndex;
		applier.UpdateStats();

	}

	public void UpgradeSpeed(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.speedLevel ++;
		applier.UpdateStats();

	}

	public void UpgradeHandling(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.handlingLevel ++;
		applier.UpdateStats();

	}

	public void UpgradeBrake(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.brakeLevel ++;
		applier.UpdateStats();

	}

	public void UpgradeSiren(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.isSirenPurchased = true;
		applier.UpdateStats();
		applier.ToggleSiren();

	}

	public void UpgradeNOS(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.isNOSPurchased = true;
		applier.UpdateStats();

	}

	public void UpgradeTurbo(){

		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		applier.isTurboPurchased = true;
		applier.UpdateStats();

	}

	public void BuyProperty(int price, string prefsKey){

		int playerCoins = PlayerPrefs.GetInt("Currency");

		PlayerPrefs.SetInt("Currency", playerCoins - price);
		PlayerPrefs.SetInt(prefsKey, 1);

	}

}
