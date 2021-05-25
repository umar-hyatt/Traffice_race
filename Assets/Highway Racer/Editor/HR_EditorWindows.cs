using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class HR_EditorWindows : Editor {

	[MenuItem("Highway Racer/General Settings", false, -100)]
	public static void OpenGeneralSettings(){
		Selection.activeObject =HR_HighwayRacerProperties.Instance;
	}

	[MenuItem("Highway Racer/Quick Switch To Desktop", false, -50)]
	public static void QuickSwitchToDesktop(){

		Selection.activeObject = RCC_Settings.Instance;
		RCC_Settings.Instance.toolbarSelectedIndex = 0;
		RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Keyboard;
		EditorUtility.SetDirty (RCC_Settings.Instance);
		SetEnabled("HR_POSTPROCESS", true);
		if(EditorUtility.DisplayDialog("Switched Build", "RCC Controller has been switched to Keyboard mode, and Post Processing Image Effects have been enabled.", "Ok")) {
			Selection.activeObject = HR_HighwayRacerProperties.Instance;
			HR_HighwayRacerProperties.Instance.usePostProcessingImageEffects = true;
		}
		EditorUtility.SetDirty (HR_HighwayRacerProperties.Instance);

	}

	[MenuItem("Highway Racer/Quick Switch To Mobile", false, -50)]
	public static void QuickSwitchToMobile(){

		Selection.activeObject = RCC_Settings.Instance;
		RCC_Settings.Instance.toolbarSelectedIndex = 1;
		RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Mobile;
		EditorUtility.SetDirty (RCC_Settings.Instance);
		SetEnabled("HR_POSTPROCESS", false);
		if (EditorUtility.DisplayDialog ("Switched Build", "RCC Controller has been switched to Mobile mode, and Post Processing Image Effects have been disabled.", "Ok")) {
			Selection.activeObject = HR_HighwayRacerProperties.Instance;
			HR_HighwayRacerProperties.Instance.usePostProcessingImageEffects = false;
		}
		EditorUtility.SetDirty (HR_HighwayRacerProperties.Instance);

	}

	[MenuItem("Highway Racer/Configure Player Cars", false, 1)]
	public static void OpenCarSettings(){
		Selection.activeObject =HR_PlayerCars.Instance;
	}

	[MenuItem("Highway Racer/Configure Upgradable Wheels", false, 1)]
	public static void OpenWheelsSettings(){
		Selection.activeObject =HR_Wheels.Instance;
	}

	[MenuItem("Highway Racer/PDF Documentation", false, 2)]
	public static void OpenDocs(){
		string url = "https://dl.dropboxusercontent.com/u/248930654/_Documentations/Highway%20Racer%20Complete%20Project.pdf";
		Application.OpenURL(url);
	}

	[MenuItem("Highway Racer/Highlight Traffic Cars Folder", false, 102)]
	public static void OpenTrafficCarsFolder(){
		Selection.activeObject = AssetDatabase.LoadMainAssetAtPath ("Assets/Highway Racer/Resources/TrafficCars");
	}

	[MenuItem("Highway Racer/Help", false, 1000)]
	static void Help(){

		EditorUtility.DisplayDialog("Contact", "Please include your invoice number while sending a contact form.", "Ok");

		string url = "http://www.bonecrackergames.com/contact/";
		Application.OpenURL (url);

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
