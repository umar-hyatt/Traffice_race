//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
#if HR_POSTPROCESS
using UnityEngine.PostProcessing;
#endif

public class HR_ImageEffects : MonoBehaviour {

	#if HR_POSTPROCESS
	public PostProcessingBehaviour postProcessingBehavior;
	#endif
	private HR_DirectionalLight_RT directionalLightRT;
	
	void Awake () {

		if(!PlayerPrefs.HasKey("MotionBlur"))
			PlayerPrefs.SetInt("MotionBlur", (HR_HighwayRacerProperties.Instance._defaultBlur == true ? 1 : 0));

		if(!PlayerPrefs.HasKey("Bloom"))
			PlayerPrefs.SetInt("Bloom", (HR_HighwayRacerProperties.Instance._defaultBloom == true ? 1 : 0));

		if(!PlayerPrefs.HasKey("AO"))
			PlayerPrefs.SetInt("AO", (HR_HighwayRacerProperties.Instance._defaultAO == true ? 1 : 0));

		if(!PlayerPrefs.HasKey("HQLights"))
			PlayerPrefs.SetInt("HQLights", (HR_HighwayRacerProperties.Instance._defaultHQLights == true ? 1 : 0));

		#if HR_POSTPROCESS
		postProcessingBehavior = GetComponent<PostProcessingBehaviour> ();
		#endif

		directionalLightRT = GameObject.FindObjectOfType<HR_DirectionalLight_RT> ();

		Check ();
	
	}
	
	public void Check () {

		if (directionalLightRT && PlayerPrefs.GetInt ("HQLights") == 0)
			directionalLightRT.gameObject.SetActive (false);

		if(directionalLightRT && PlayerPrefs.GetInt("HQLights") == 1)
			directionalLightRT.gameObject.SetActive (true);

		#if HR_POSTPROCESS
		if(!postProcessingBehavior){
			
			Debug.LogError ("Post Processing Behaviour not found for image effects!");
			return;

		}

		if(!postProcessingBehavior.enabled)
			postProcessingBehavior.enabled = true;

		if (Application.isMobilePlatform) {

			Debug.LogWarning ("Post Processing is not supported by mobile devices. Disabling image effects.");
			Destroy (postProcessingBehavior);

		}

		PostProcessingProfile profile = postProcessingBehavior.profile;

		if(profile && PlayerPrefs.GetInt("Bloom") == 0)
			profile.bloom.enabled = false;
		
		if(profile && PlayerPrefs.GetInt("Bloom") == 1)
			profile.bloom.enabled = true;

		if(profile && PlayerPrefs.GetInt("MotionBlur") == 0)
			profile.motionBlur.enabled = false;
		
		if(profile && PlayerPrefs.GetInt("MotionBlur") == 1)
			profile.motionBlur.enabled = true;

		if(profile && PlayerPrefs.GetInt("AO") == 0)
			profile.ambientOcclusion.enabled = false;
		
		if(profile && PlayerPrefs.GetInt("AO") == 1)
			profile.ambientOcclusion.enabled = true;
		#endif

	}

}
