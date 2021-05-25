//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_Watermark : MonoBehaviour {

	static HR_Watermark instance;

	void Start () {

		if (instance){
			Destroy (gameObject);
		}else{
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	
	}

}
