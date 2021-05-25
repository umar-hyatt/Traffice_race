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

public class HR_GraphicsLevel : MonoBehaviour {

	public graphicsLevel _graphicsLevel;
	public enum graphicsLevel{Low, Medium, High}
	private Button button;
	private Color defButtonColor;

	void Awake(){

		button = GetComponent<Button>();
		defButtonColor = button.image.color;

	}

	public void OnClick () {
		
		switch(_graphicsLevel){

		case graphicsLevel.Low:
			QualitySettings.SetQualityLevel(0);
			break;
		case graphicsLevel.Medium:
			QualitySettings.SetQualityLevel(1);
			break;
		case graphicsLevel.High:
			QualitySettings.SetQualityLevel(2);
			break;

		}

	}

	void Update(){

		button.image.color = defButtonColor;
		Color activeColor = new Color(.667f, 1f, 0f);

		if(QualitySettings.GetQualityLevel() == 0 && _graphicsLevel == graphicsLevel.Low)
			button.image.color = activeColor;

		if(QualitySettings.GetQualityLevel() == 1 && _graphicsLevel == graphicsLevel.Medium)
			button.image.color = activeColor;

		if(QualitySettings.GetQualityLevel() == 2 && _graphicsLevel == graphicsLevel.High)
			button.image.color = activeColor;

	}

}
