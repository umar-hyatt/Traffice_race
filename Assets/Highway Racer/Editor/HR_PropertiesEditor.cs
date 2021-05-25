using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HR_HighwayRacerProperties))]
public class HR_PropertiesEditor : Editor {

	HR_HighwayRacerProperties prop;

	Vector2 scrollPos;
	GameObject[] playerCars;
	GameObject[] upgradableWheels;

	Color orgColor;

	private int _Width = 500;
	public int Width {
		get {
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			Rect scale = GUILayoutUtility.GetLastRect();

			if(scale.width != 1) {
				_Width = System.Convert.ToInt32(scale.width);
			}

			return _Width;
		}
	}

	public bool usePostProcessingImageEffects{
		
		get{
			
			bool _bool = prop.usePostProcessingImageEffects;

			return _bool;

		}

		set{
			
			bool _bool = prop.usePostProcessingImageEffects;

			if(_bool == value)
				return;

			prop.usePostProcessingImageEffects = value;

			SetEnabled("HR_POSTPROCESS", value);

		}

	}

	public override void OnInspectorGUI (){

		serializedObject.Update();
		prop = (HR_HighwayRacerProperties)target;
		orgColor = GUI.color;
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Highway Racer Properties Editor Window", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project. Don't change directory of the ''Resources/HR_Assets''.", EditorStyles.helpBox);
		EditorGUILayout.Space();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false );

		EditorGUIUtility.labelWidth = 180f;

		GUILayout.Label("General Settings", EditorStyles.boldLabel);

		prop._minimumSpeedForGainScore = EditorGUILayout.IntField("Min Speed For Gain Score", prop._minimumSpeedForGainScore);
		prop._minimumSpeedForHighSpeed = EditorGUILayout.IntField("Min Speed For High Speed", prop._minimumSpeedForHighSpeed);
		prop._minimumCollisionForGameOver = EditorGUILayout.IntField("Min Col For Game Over", prop._minimumCollisionForGameOver);

		EditorGUILayout.Space(); 

		GUILayout.Label("Default Config At First Initialization", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("Default settings when there are no any saved PlayerPrefs data. At first init only on target platform.", MessageType.Info, true);

		EditorGUILayout.Space(); 
		prop._defaultBodyColor = EditorGUILayout.ColorField("Default Body Color", prop._defaultBodyColor);
		EditorGUILayout.Space();

		usePostProcessingImageEffects = EditorGUILayout.BeginToggleGroup ("Use Post Processing Image Effects", usePostProcessingImageEffects);

		if(!usePostProcessingImageEffects)
			EditorGUILayout.LabelField("Game will not use Post Processing Image effects. You may want to delete ''PostProcessing'' folder from your project to avoid unnecessary assets.", EditorStyles.helpBox);
		else
			EditorGUILayout.LabelField("Game will use Post Processing Image effects. This can be very heavy for mobile devices. Ensure that your project has ''PostProcessing''. Highway Racer already includes it. But you can import it from Asset Store. Restart Unity after enabling this.", EditorStyles.helpBox);

		EditorGUI.indentLevel++;
		prop._defaultBloom = EditorGUILayout.ToggleLeft("Bloom Enabled By Default", prop._defaultBloom, GUILayout.ExpandWidth(true));
		prop._defaultAO = EditorGUILayout.ToggleLeft("AO Enabled By Default", prop._defaultAO, GUILayout.ExpandWidth(true));
		prop._defaultBlur = EditorGUILayout.ToggleLeft("Motion Blur Enabled By Default", prop._defaultBlur, GUILayout.ExpandWidth(true));
		EditorGUI.indentLevel--;
		EditorGUILayout.EndToggleGroup ();

		EditorGUILayout.Space();
		prop._defaultHQLights = EditorGUILayout.ToggleLeft("HQ Lights Enabled By Default", prop._defaultHQLights, GUILayout.ExpandWidth(true));
		prop._shakeCamera = EditorGUILayout.ToggleLeft("Camera Shake", prop._shakeCamera, GUILayout.ExpandWidth(true));
		prop._tiltCamera = EditorGUILayout.ToggleLeft("Camera Tilt", prop._tiltCamera, GUILayout.ExpandWidth(true));

		EditorGUILayout.Space();

		GUILayout.Label("Score Multipliers", EditorStyles.boldLabel);

		prop._totalDistanceMoneyMP = EditorGUILayout.IntField("Total Distance MP", prop._totalDistanceMoneyMP, GUILayout.ExpandWidth(true));
		prop._totalNearMissMoneyMP = EditorGUILayout.IntField("Total Near Miss MP", prop._totalNearMissMoneyMP, GUILayout.ExpandWidth(true));
		prop._totalOverspeedMoneyMP = EditorGUILayout.IntField("Total Over Speed MP", prop._totalOverspeedMoneyMP, GUILayout.ExpandWidth(true));
		prop._totalOppositeDirectionMP = EditorGUILayout.IntField("Total Opposite Direction MP", prop._totalOppositeDirectionMP, GUILayout.ExpandWidth(true));

		EditorGUILayout.Space();

		GUILayout.Label("Sound Effects", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenuClips"), new GUIContent("Main Menu Soundtracks"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gameplayClips"), new GUIContent("GamePlay Soundtracks"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonClickAudioClip"), new GUIContent("Button Click SFX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("nearMissAudioClip"), new GUIContent("Near Miss SFX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("labelSlideAudioClip"), new GUIContent("Label Slide SFX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("countingPointsAudioClip"), new GUIContent("Counting Points SFX"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("bombTimerAudioClip"), new GUIContent("Bomb Timer Beep SFX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("sirenAudioClip"), new GUIContent("Police Siren SFX"));

		EditorGUILayout.Space();

		GUI.color = new Color(.5f, 1f, 1f, 1f);
		GUILayout.Label("Select Main Controller Type", EditorStyles.boldLabel);

		if (GUILayout.Button ("Switch Controller from RCC Settings"))
			Selection.activeObject = RCC_Settings.Instance;
		
		GUI.color = orgColor;
		EditorGUILayout.Space();

		prop._1MMoneyForTesting = EditorGUILayout.ToggleLeft("1M Money For Testing", prop._1MMoneyForTesting, GUILayout.ExpandWidth(true));

		EditorGUILayout.Space();

		GUILayout.Label("Selectable Player Cars", EditorStyles.boldLabel);

		playerCars = new GameObject[HR_PlayerCars.Instance.cars.Length];
		
		for (int i = 0; i < playerCars.Length; i++) {
			playerCars[i] = HR_PlayerCars.Instance.cars[i].playerCar;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.ObjectField("Player Car " + i, playerCars[i], typeof(GameObject), false);
			EditorGUILayout.EndVertical();
		}

		GUI.color = Color.cyan;

		if(GUILayout.Button("Configure Player Cars")){
			Selection.activeObject = Resources.Load("HR_Assets/HR_PlayerCars") as HR_PlayerCars;
		}

		GUI.color = orgColor;

		EditorGUILayout.Space();

		GUILayout.Label("Upgradable Wheels", EditorStyles.boldLabel);

		upgradableWheels = new GameObject[HR_Wheels.Instance.wheels.Length];
		
		for (int i = 0; i < upgradableWheels.Length; i++) {
			upgradableWheels[i] = HR_Wheels.Instance.wheels[i].wheel;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.ObjectField("Upgradable Wheels " + i, upgradableWheels[i], typeof(GameObject), false);
			EditorGUILayout.EndVertical();
		}

		GUI.color = Color.cyan;

		if(GUILayout.Button("Configure Upgradable Wheels")){
			Selection.activeObject = Resources.Load("HR_Assets/HR_Wheels") as HR_Wheels;
		}

		GUI.color = orgColor;

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("attachableSiren"), new GUIContent("Attachable Siren"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("explosionEffect"), new GUIContent("Explosion Particle Effects"));

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("trafficCarsLayer"), new GUIContent("Layer Of The Traffic Cars"));

		EditorGUILayout.EndScrollView();

		EditorGUILayout.Space();

		GUI.color = Color.red;

		if(GUILayout.Button("Reset To Defaults")){
			if(EditorUtility.DisplayDialog("Delete All PlayerPrefs Data", "Are you sure to reset all settings to default? Don't worry, your cars, prices, upgrades will stay the same.", "Yes", "Nope")){
				ResetToDefaults();
				Debug.Log("Resetted To Defaults!");
			}
		}
		if(GUILayout.Button("Delete All PlayerPrefs")){
			if(EditorUtility.DisplayDialog("Delete All PlayerPrefs Data", "Are you sure to delete all local playerprefs data?", "Yes", "Nope")){
				PlayerPrefs.DeleteAll();
				Debug.Log("Deleted All PlayerPrefs Data!");
				PlayerPrefs.SetInt("Installed", 1);
			}
		}

		GUI.color = Color.cyan;

		if(GUILayout.Button("Open PDF Documentation")){
			string url = "http://www.bonecrackergames.com/highway-racer/";
			Application.OpenURL(url);
		}

		GUI.color = orgColor;

		EditorGUILayout.LabelField("Highway Racer V2.3\nCreated by Buğra Özdoğanlar\nBoneCrackerGames", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

		serializedObject.ApplyModifiedProperties();

		if(GUI.changed)
			EditorUtility.SetDirty(prop);

	}

	void ResetToDefaults(){

		prop._minimumSpeedForGainScore = 80;
		prop._minimumSpeedForHighSpeed = 100;
		prop._minimumCollisionForGameOver = 5;
		prop._defaultBodyColor = Color.red + (Color.green / 2f);

		prop._defaultBloom = false;
		prop._defaultAO = false;
		prop._defaultBlur = false;
		prop._1MMoneyForTesting = false;

		prop._totalDistanceMoneyMP = 120;
		prop._totalNearMissMoneyMP = 10;
		prop._totalOverspeedMoneyMP = 5;
		prop._totalOppositeDirectionMP = 5;

	}

	private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
	{

		BuildTargetGroup.Standalone,
		BuildTargetGroup.Android,
		BuildTargetGroup.iOS,
		BuildTargetGroup.WebGL,
		BuildTargetGroup.Facebook,
		BuildTargetGroup.N3DS,
		BuildTargetGroup.XboxOne,
		BuildTargetGroup.PS4,
		BuildTargetGroup.PSP2,
		BuildTargetGroup.PSM,
		BuildTargetGroup.tvOS,
		BuildTargetGroup.SamsungTV,
		BuildTargetGroup.Tizen,
		BuildTargetGroup.Switch,
		BuildTargetGroup.WiiU,
		BuildTargetGroup.WSA

	};

	private static void SetEnabled(string defineName, bool enable)
	{
		//Debug.Log("setting "+defineName+" to "+enable);
		foreach (var group in buildTargetGroups)
		{
			var defines = GetDefinesList(group);
			if (enable)
			{
				if (defines.Contains(defineName))
				{
					return;
				}
				defines.Add(defineName);
			}
			else
			{
				if (!defines.Contains(defineName))
				{
					return;
				}
				while (defines.Contains(defineName))
				{
					defines.Remove(defineName);
				}
			}
			string definesString = string.Join(";", defines.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
		}
	}

	private static List<string> GetDefinesList(BuildTargetGroup group)
	{
		return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
	}

}
