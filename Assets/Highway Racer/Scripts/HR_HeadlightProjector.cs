//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_HeadlightProjector : MonoBehaviour {

	private HR_GamePlayHandler gpHandler;
	private Projector projector;
	private Light headlight;

	void Start () {

		gpHandler = HR_GamePlayHandler.Instance;
		projector = GetComponent<Projector>();
		headlight = GetComponentInParent<Light>();

		Material newMaterial = new Material(projector.material);
		projector.material = newMaterial ;
	
	}

	void Update () {

		if(!headlight.enabled){
			projector.enabled = false;
			return;
		}else{
			projector.enabled = true;
		}

		if(gpHandler && gpHandler.dayOrNight == HR_GamePlayHandler.DayOrNight.Day)
			projector.material.color = headlight.color * headlight.intensity * .05f;
		else
			projector.material.color = headlight.color * headlight.intensity * .25f;

		projector.farClipPlane = Mathf.Lerp(10f, 40f, (headlight.range - 50) / 150);
		projector.fieldOfView = Mathf.Lerp(40f, 30f, (headlight.range - 50) / 150);
	
	}

}
