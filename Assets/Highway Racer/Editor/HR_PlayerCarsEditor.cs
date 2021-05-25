using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HR_PlayerCars))]
public class HR_PlayerCarsEditor : Editor {

	HR_PlayerCars prop;

	Vector2 scrollPos;
	List<HR_PlayerCars.Cars> playerCars = new List<HR_PlayerCars.Cars>();

	Color orgColor;

	[MenuItem("Highway Racer/Highlight Player Cars Folder", false, 100)]
	public static void OpenPlayerCarsFolder(){
		Selection.activeObject = HR_PlayerCars.Instance;
	}

	public override void OnInspectorGUI (){

		serializedObject.Update();
		prop = (HR_PlayerCars)target;
		orgColor = GUI.color;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Player Cars Editor", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project. Don't change directory of the ''Resources/HR_Assets''.", EditorStyles.helpBox);
		EditorGUILayout.Space();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false );

		EditorGUIUtility.labelWidth = 110f;
		//EditorGUIUtility.fieldWidth = 10f;

		GUILayout.Label("Player Cars", EditorStyles.boldLabel);

		EditorGUI.indentLevel ++;

		for (int i = 0; i < prop.cars.Length; i++) {

			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.Space();

			if(prop.cars[i].playerCar)
				EditorGUILayout.LabelField(prop.cars[i].playerCar.name, EditorStyles.boldLabel);

			EditorGUILayout.Space();
			prop.cars[i].playerCar = (GameObject)EditorGUILayout.ObjectField("Player Car Prefab", prop.cars[i].playerCar, typeof(GameObject), false, GUILayout.MaxWidth(475f));
			EditorGUILayout.Space();

			if(prop.cars[i].playerCar && prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>()){

				if (prop.cars [i].playerCar.GetComponent<HR_ModApplier> () == null)
					prop.cars [i].playerCar.AddComponent<HR_ModApplier> ();

				if (prop.cars [i].playerCar.GetComponent<HR_PlayerHandler> () == null)
					prop.cars [i].playerCar.AddComponent<HR_PlayerHandler> ();
				
				EditorGUILayout.BeginHorizontal();
				prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().maxspeed = EditorGUILayout.FloatField("Speed", prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().maxspeed, GUILayout.MaxWidth(150f));		prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwaySteeringHelper = EditorGUILayout.FloatField("Handling", prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwaySteeringHelper, GUILayout.MaxWidth(150f));		prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwayBrakingHelper = EditorGUILayout.FloatField("Braking", prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwayBrakingHelper, GUILayout.MaxWidth(150f));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeSpeed = EditorGUILayout.FloatField("Max Speed", Mathf.Clamp(prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeSpeed, prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().maxspeed, 360f), GUILayout.MaxWidth(150f));	prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeHandling  = EditorGUILayout.FloatField("Max Handling", Mathf.Clamp(prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeHandling, prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwaySteeringHelper, 10f), GUILayout.MaxWidth(150f));	prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeBrake = EditorGUILayout.FloatField("Max Braking",  Mathf.Clamp(prop.cars[i].playerCar.GetComponent<HR_ModApplier>().maxUpgradeBrake, prop.cars[i].playerCar.GetComponent<RCC_CarControllerV3>().highwayBrakingHelper, 10f), GUILayout.MaxWidth(150f));

				if(GUI.changed)
					EditorUtility.SetDirty (prop.cars [i].playerCar);
				
				EditorGUILayout.EndHorizontal();

			}else{
				
				EditorGUILayout.HelpBox("Select A RCC Based Car", MessageType.Error);

			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			if(prop.cars[i].price <= 0)
				prop.cars[i].unlocked = true;

			if (prop.cars != null && prop.cars [i] != null && prop.cars [i].playerCar) {

				prop.cars [i].playerCar.GetComponent<RCC_CarControllerV3> ().engineTorque = EditorGUILayout.FloatField ("Engine Torque", prop.cars [i].playerCar.GetComponent<RCC_CarControllerV3> ().engineTorque, GUILayout.MaxWidth (150f));
				prop.cars [i].price = EditorGUILayout.IntField ("Price", prop.cars [i].price, GUILayout.MaxWidth (150f));
				prop.cars [i].unlocked = EditorGUILayout.ToggleLeft ("Unlocked", prop.cars [i].unlocked, GUILayout.MaxWidth (122f));

			}

			if (GUILayout.Button ("\u2191", GUILayout.MaxWidth (25f))) {
				Up (i);
			}

			if (GUILayout.Button ("\u2193", GUILayout.MaxWidth (25f))) {
				Down (i);
			}

			GUI.color = Color.red;

			if (GUILayout.Button ("X", GUILayout.MaxWidth (25f))) {
				RemoveCar (i);
			}

			GUI.color = orgColor;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Bombed Car For Bomb Mode", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("bombedVehicleForBombMode"), new GUIContent("Bombed Car"), false);
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();

		GUI.color = Color.cyan;

		if(GUILayout.Button("Create Player Car")){

			AddNewCar();

		}

		if(GUILayout.Button("--< Return To General Settings")){

			OpenGeneralSettings();

		}

		GUI.color = orgColor;

		EditorGUILayout.EndScrollView();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Highway Racer V2.3\nCreated by Buğra Özdoğanlar\nBoneCrackerGames", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

		serializedObject.ApplyModifiedProperties();

		if(GUI.changed)
			EditorUtility.SetDirty(prop);

	}

	void AddNewCar(){

		playerCars.Clear();
		playerCars.AddRange(prop.cars);
		HR_PlayerCars.Cars newCar = new HR_PlayerCars.Cars();
		playerCars.Add(newCar);
		prop.cars = playerCars.ToArray();
		PlayerPrefs.SetInt("SelectedPlayerCarIndex", 0);

	}

	void RemoveCar(int index){

		playerCars.Clear();
		playerCars.AddRange(prop.cars);
		playerCars.RemoveAt(index);
		prop.cars = playerCars.ToArray();
		PlayerPrefs.SetInt("SelectedPlayerCarIndex", 0);

	}

	void Up(int index){

		if (index <= 0)
			return;

		playerCars.Clear();
		playerCars.AddRange(prop.cars);

		HR_PlayerCars.Cars currentCar = playerCars [index];
		HR_PlayerCars.Cars previousCar = playerCars [index - 1];

		playerCars.RemoveAt (index);
		playerCars.RemoveAt (index - 1);

		playerCars.Insert (index - 1, currentCar);
		playerCars.Insert (index, previousCar);

		prop.cars = playerCars.ToArray();
		PlayerPrefs.SetInt("SelectedPlayerCarIndex", 0);

	}

	void Down(int index){

		if (index >= prop.cars.Length - 1)
			return;

		playerCars.Clear();
		playerCars.AddRange(prop.cars);

		//		foreach(HR_PlayerCars.Cars qwe in playerCars)
		//			Debug.Log(qwe.playerCar.name);

		HR_PlayerCars.Cars currentCar = playerCars [index];
		HR_PlayerCars.Cars nextCar = playerCars [index + 1];

		playerCars.RemoveAt (index);
		playerCars.Insert (index, nextCar);

		playerCars.RemoveAt (index + 1);
		playerCars.Insert (index + 1, currentCar);

		prop.cars = playerCars.ToArray();
		PlayerPrefs.SetInt("SelectedPlayerCarIndex", 0);

	}

	void OpenGeneralSettings(){

		Selection.activeObject = HR_HighwayRacerProperties.Instance;

	}

}
