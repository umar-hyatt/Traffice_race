//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[System.Serializable]
public class HR_PlayerCars : ScriptableObject {

	public static HR_PlayerCars instance;
	public static HR_PlayerCars Instance
	{
		get
		{
			if(instance == null)
				instance = Resources.Load("HR_Assets/HR_PlayerCars") as HR_PlayerCars;
			return instance;
		}
		
	}

	[Space(10f)]

	public GameObject bombedVehicleForBombMode;

	[System.Serializable]
	public class Cars{

		public GameObject playerCar;

		public float maxSpeed = 200f;
		public float maxHandling = 4f;
		public float maxBraking = 4f;

		public bool unlocked = false;
		public int price = 25000;

	}

	public Cars[] cars;

}
