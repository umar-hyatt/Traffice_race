//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HR_ModificationUpgrade : MonoBehaviour {

	public UpgradeClass upgradeClass;
	public enum UpgradeClass{Speed, Handling, Brake, Siren, NOS, Turbo}
	[HideInInspector]public HR_ModApplier applier;
	public int upgradePrice;
	public bool fullyUpgraded = false;
	public Text priceLabel;
	private Image priceImage;

	void Awake () {

		priceImage = priceLabel.GetComponentInParent<Image>();
		
	}

	void OnEnable(){

		applier = GameObject.FindObjectOfType<HR_ModApplier>();

	}
	
	public void OnClick () {

		int playerCoins = PlayerPrefs.GetInt("Currency");

		if(fullyUpgraded && upgradeClass == UpgradeClass.Siren){
			GameObject.FindObjectOfType<HR_ModHandler>().UpgradeSiren();
		}

		if(fullyUpgraded && upgradeClass == UpgradeClass.NOS){
			GameObject.FindObjectOfType<HR_ModHandler>().UpgradeNOS();
		}

		if(fullyUpgraded && upgradeClass == UpgradeClass.Turbo){
			GameObject.FindObjectOfType<HR_ModHandler>().UpgradeTurbo();
		}
		
		if(playerCoins < upgradePrice)
			return;
		
		if(!fullyUpgraded)
			BuyUpgrade();
		
	}

	void Update(){

		switch(upgradeClass){
			
		case UpgradeClass.Speed:
			if(applier.speedLevel >= 5){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
		case UpgradeClass.Handling:
			if(applier.handlingLevel >= 5){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
		case UpgradeClass.Brake:
			if(applier.brakeLevel >= 5){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
		case UpgradeClass.Siren:
			if(applier.isSirenPurchased == true){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
		case UpgradeClass.NOS:
			if(applier.isNOSPurchased == true){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
		case UpgradeClass.Turbo:
			if(applier.isTurboPurchased == true){
				fullyUpgraded = true;
			}else{
				fullyUpgraded = false;
			}
			break;
			
		}
		
		if(!fullyUpgraded){
			if(!priceImage.gameObject.activeSelf)
				priceImage.gameObject.SetActive(true);
			if(priceLabel.text != upgradePrice.ToString())
				priceLabel.text = upgradePrice.ToString();
		}else{
			if(priceImage.gameObject.activeSelf)
				priceImage.gameObject.SetActive(false);
			if(priceLabel.text != "UPGRADED")
				priceLabel.text = "UPGRADED";
		}

	}
	
	void BuyUpgrade(){
		
		int playerCoins = PlayerPrefs.GetInt("Currency");
		HR_ModApplier applier = GameObject.FindObjectOfType<HR_ModApplier>();
		HR_ModHandler handler = GameObject.FindObjectOfType<HR_ModHandler>();
		
		if(playerCoins >= upgradePrice){
			
			switch(upgradeClass){

			case UpgradeClass.Speed:
				if(applier.speedLevel < 5){
					handler.UpgradeSpeed();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;
			case UpgradeClass.Handling:
				if(applier.handlingLevel < 5){
					handler.UpgradeHandling();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;
			case UpgradeClass.Brake:
				if(applier.brakeLevel < 5){
					handler.UpgradeBrake();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;
			case UpgradeClass.Siren:
				if(applier.isSirenPurchased == false){
					handler.UpgradeSiren();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;
			case UpgradeClass.NOS:
				if(applier.isNOSPurchased == false){
					handler.UpgradeNOS();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;
			case UpgradeClass.Turbo:
				if(applier.isTurboPurchased == false){
					handler.UpgradeTurbo();
					PlayerPrefs.SetInt("Currency", playerCoins - upgradePrice);
				}
				break;

			}

		}
		
	}

}
