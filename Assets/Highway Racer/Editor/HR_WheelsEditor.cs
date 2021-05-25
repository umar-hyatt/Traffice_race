using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HR_Wheels))]
public class HR_WheelsEditor : Editor {

	HR_Wheels prop;

	Vector2 scrollPos;
	List<HR_Wheels.Wheels> playerCars = new List<HR_Wheels.Wheels>();

	Color orgColor;

	[MenuItem("Highway Racer/Highlight Wheels Folder", false, 101)]
	public static void OpenWheelsFolder(){
		Selection.activeObject = AssetDatabase.LoadMainAssetAtPath ("Assets/Highway Racer/Resources/Wheels");
	}

	public override void OnInspectorGUI (){

		serializedObject.Update();
		prop = (HR_Wheels)target;
		orgColor = GUI.color;
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Wheels Editor", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project. Don't change directory of the ''Resources/HR_Assets''.", EditorStyles.helpBox);
		EditorGUILayout.Space();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false );

		EditorGUIUtility.labelWidth = 110f;
		//EditorGUIUtility.fieldWidth = 10f;

		GUILayout.Label("Wheels", EditorStyles.boldLabel);

		for (int i = 0; i < prop.wheels.Length; i++) {

			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.Space();

			if(prop.wheels[i].wheel)
				EditorGUILayout.LabelField(prop.wheels[i].wheel.name, EditorStyles.boldLabel);

			EditorGUILayout.Space();
			prop.wheels[i].wheel = (GameObject)EditorGUILayout.ObjectField("Wheel Prefab", prop.wheels[i].wheel, typeof(GameObject), false);
			EditorGUILayout.Space();

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			if(prop.wheels[i].price <= 0)
				prop.wheels[i].unlocked = true;

			prop.wheels[i].unlocked = EditorGUILayout.ToggleLeft("Unlocked", prop.wheels[i].unlocked, GUILayout.Width(150f));		prop.wheels[i].price = EditorGUILayout.IntField("Price", prop.wheels[i].price, GUILayout.Width(150f));		GUI.color = Color.red;		if(GUILayout.Button("Remove")){RemoveWheel(i);}	GUI.color = orgColor;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

		}

		GUI.color = Color.cyan;

		if(GUILayout.Button("Create Wheel")){

			AddNewWheel();

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

	void AddNewWheel(){

		playerCars.Clear();
		playerCars.AddRange(prop.wheels);
		HR_Wheels.Wheels newCar = new HR_Wheels.Wheels();
		playerCars.Add(newCar);
		prop.wheels = playerCars.ToArray();

	}

	void RemoveWheel(int index){

		playerCars.Clear();
		playerCars.AddRange(prop.wheels);
		playerCars.RemoveAt(index);
		prop.wheels = playerCars.ToArray();

	}

	void OpenGeneralSettings(){

		Selection.activeObject =HR_HighwayRacerProperties.Instance;

	}

}
