using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HR_InitOnLoad {
	
	static HR_InitOnLoad(){
		
		if(!EditorPrefs.HasKey("HRV2.3_Installed")){
			
			EditorPrefs.SetInt("HRV2.3_Installed", 1);
			EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing Highway Racer Complete Project. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started");
			EditorUtility.DisplayDialog("Current Controller Type", "Current controller type is ''Mobile''. You can swith it from Highway Racer --> Switch to Keyboard / Mobile.", "Ok");
			EditorUtility.DisplayDialog("Post Processing Image Effects", "You can enable / disable to use Post Processing Image Effects from Highway Racer --> General Settings.", "Ok");
			Selection.activeObject = HR_HighwayRacerProperties.Instance;

		}

	}

}