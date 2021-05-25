//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_Viewport : MonoBehaviour {

	private Transform target;
	internal bool leftSide = false;

	void LateUpdate () {

		if(!target){
			target = GameObject.FindObjectOfType<RCC_CarControllerV3>().transform;
			return;
		}

		transform.position = Camera.main.WorldToScreenPoint(leftSide ? new Vector3(target.position.x - 1f, target.position.y, target.position.z) : new Vector3(target.position.x + 1, target.position.y, target.position.z));
	
	}

}
