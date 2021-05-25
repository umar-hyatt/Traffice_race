//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;

public class RCC_InitLoad : MonoBehaviour {

	[InitializeOnLoad]
	public class InitOnLoad {

		static InitOnLoad(){
			
			if(!EditorPrefs.HasKey("RCC" + "V3.2b" + "Installed")){
				
				EditorPrefs.SetInt("RCC" + "V3.2b" + "Installed", 1);
				EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing and using Realistic Car Controller. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started");

				if(EditorUtility.DisplayDialog("Importing BoneCracker Games Shared Assets", "Do you want to import ''BoneCracker Games Shared Assets'' to your project? It will be used for enter / exit on all vehicles created by BoneCracker Games in future.", "Import it", "No"))
					AssetDatabase.ImportPackage("Assets/RealisticCarControllerV3/For BCG Shared Assets/BCG Shared Assets.unitypackage", true);

				Selection.activeObject = RCC_Settings.Instance;

			}

		}

	}

}
