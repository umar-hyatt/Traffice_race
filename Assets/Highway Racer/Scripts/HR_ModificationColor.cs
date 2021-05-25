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
using System.Collections.Generic;

public class HR_ModificationColor : MonoBehaviour {

	public pickedColor _pickedColor;
	public enum pickedColor{Orange, Red, Green, Blue, Yellow, Black, White, Cyan, Magenta, Pink}
	public int colorPrice;
	public bool unlocked = false;
	private Text priceLabel;
	private Image priceImage;

	void Start(){

		priceLabel = GetComponentInChildren<Text>();
		priceImage = priceLabel.GetComponentInParent<Image>();

	}

	public void OnClick () {

		if(!unlocked){
			BuyColor();
			return;
		}

		HR_ModHandler handler = GameObject.FindObjectOfType<HR_ModHandler>();
		Color selectedColor = new Color();

		switch(_pickedColor){

		case pickedColor.Orange:
			selectedColor = Color.red + (Color.green / 2f);
			break;

		case pickedColor.Red:
			selectedColor = Color.red;
			break;

		case pickedColor.Green:
			selectedColor = Color.green;
			break;

		case pickedColor.Blue:
			selectedColor = Color.blue;
			break;

		case pickedColor.Yellow:
			selectedColor = Color.yellow;
			break;

		case pickedColor.Black:
			selectedColor = Color.black;
			break;

		case pickedColor.White:
			selectedColor = Color.white;
			break;

		case pickedColor.Cyan:
			selectedColor = Color.cyan;
			break;

		case pickedColor.Magenta:
			selectedColor = Color.magenta;
			break;

		case pickedColor.Pink:
			selectedColor = new Color(1, 0f, .5f);
			break;

		}

		handler.ChangeChassisColor(selectedColor);
	
	}
	
	void Update(){

		string currentColorString = _pickedColor.ToString();

		if(colorPrice <= 0 && !unlocked){
			PlayerPrefs.SetInt(HR_MainMenuHandler.Instance.currentCar.transform.name + "OwnedColor" + currentColorString, 1);
			unlocked = true;
		}

		if(PlayerPrefs.HasKey(HR_MainMenuHandler.Instance.currentCar.transform.name + "OwnedColor" + currentColorString))
			unlocked = true;
		else
			unlocked = false;

		if(!unlocked){
			if(!priceImage.gameObject.activeSelf)
				priceImage.gameObject.SetActive(true);
			if(priceLabel.text != colorPrice.ToString())
				priceLabel.text = colorPrice.ToString();
		}else{
			if(priceImage.gameObject.activeSelf)
				priceImage.gameObject.SetActive(false);
			if(priceLabel.text != "UNLOCKED")
				priceLabel.text = "UNLOCKED";
		}

	}

	void BuyColor(){

		int playerCoins = PlayerPrefs.GetInt("Currency");
		string currentColorString = _pickedColor.ToString();

		if(playerCoins >= colorPrice){
			HR_ModHandler handler = GameObject.FindObjectOfType<HR_ModHandler>();
			handler.BuyProperty(colorPrice, HR_MainMenuHandler.Instance.currentCar.transform.name + "OwnedColor" + currentColorString);
		}

	}

}
