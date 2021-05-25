//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_SetLightmapsManually : MonoBehaviour {

	#region SINGLETON PATTERN
	public static HR_SetLightmapsManually _instance;
	public static HR_SetLightmapsManually Instance
	{
		get{
			if (_instance == null){

				_instance = GameObject.FindObjectOfType<HR_SetLightmapsManually>();

				if (_instance == null)
				{
					GameObject container = new GameObject("SetLightmaps");
					_instance = container.AddComponent<HR_SetLightmapsManually>();
				}
			}
			
			return _instance;
		}
	}
	#endregion

	internal Renderer[] referenceRenderers;
	internal Renderer[] targetRenderers;
	
	internal Texture2D[] m_lightmapArray;

	public void AlignLightmaps(GameObject referenceMainGameObject, GameObject targetMainGameObject){

		referenceRenderers = referenceMainGameObject.GetComponentsInChildren<Renderer>();
		targetRenderers = targetMainGameObject.GetComponentsInChildren<Renderer>();

		for (int i = 0; i < targetRenderers.Length; i++) {

			targetRenderers[i].lightmapIndex = referenceRenderers[i].lightmapIndex;
			targetRenderers[i].lightmapScaleOffset = referenceRenderers[i].lightmapScaleOffset;
			
		}

	}

}
