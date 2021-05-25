//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Feeding material's emission channel for self illumin effect.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Light/RCC Light Emission")]
public class RCC_LightEmission : MonoBehaviour {

	private Light sharedLight;
	public Renderer lightRenderer;
	public int materialIndex = 0;
	public bool noTexture = false;
	public float multiplier = 1f;

	private int colorID;
	private Material material;
	private Color targetColor;

	void Start () {

		sharedLight = GetComponent<Light>();
		material = lightRenderer.materials [materialIndex];
		material.EnableKeyword("_EMISSION");
		colorID = Shader.PropertyToID("_EmissionColor");

		if (!material.HasProperty (colorID))
			enabled = false;

	}

	void Update () {

		if (!material.HasProperty (colorID))
			return;

		if(!sharedLight.enabled)
			targetColor = Color.white * 0f;

		if (!noTexture)
			targetColor = Color.white * sharedLight.intensity * multiplier;
		else
			targetColor = sharedLight.color * sharedLight.intensity * multiplier;

		if (material.GetColor (colorID) != (targetColor))
			material.SetColor (colorID, targetColor);

	}

}
