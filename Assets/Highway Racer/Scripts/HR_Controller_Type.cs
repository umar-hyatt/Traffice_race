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

public class HR_Controller_Type : MonoBehaviour {

	public controllerType _controllerType;
	public enum controllerType{ keypad, accelerometer }
	private Button sprite;
	private Color defCol;

	void Start () {

		sprite = GetComponent<Button>();
		defCol = sprite.image.color;

		if(!PlayerPrefs.HasKey("ControllerType"))
			PlayerPrefs.SetInt("ControllerType", 0);

		Check ();
	
	}
	

	public void OnClick () {
	
		if(_controllerType == controllerType.keypad){
			PlayerPrefs.SetInt("ControllerType", 0);
		}

		if(_controllerType == controllerType.accelerometer){
			PlayerPrefs.SetInt("ControllerType", 1);
		}

		HR_Controller_Type[] ct = GameObject.FindObjectsOfType<HR_Controller_Type>();

		foreach(HR_Controller_Type cts in ct){
			cts.Check();
		}

	}

	void Check(){

		if(PlayerPrefs.GetInt("ControllerType") == 0){
			if(_controllerType == controllerType.keypad){
				sprite.image.color = new Color(.667f, 1f, 0f);
			}
			if(_controllerType == controllerType.accelerometer){
				sprite.image.color = defCol;
			}
		}
		if(PlayerPrefs.GetInt("ControllerType") == 1){
			if(_controllerType == controllerType.keypad){
				sprite.image.color = defCol;
			}
			if(_controllerType == controllerType.accelerometer){
				sprite.image.color = new Color(.667f, 1f, 0f);
			}
		}

		if(GameObject.FindObjectOfType<RCC_CarControllerV3>())
			GameObject.FindObjectOfType<RCC_CarControllerV3>().GetControllerType();

	}
	
}
