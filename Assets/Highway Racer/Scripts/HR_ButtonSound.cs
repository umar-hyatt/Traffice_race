//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class HR_ButtonSound : MonoBehaviour, IPointerClickHandler{

	private AudioSource clickSound;

	public void OnPointerClick(PointerEventData data){

		if (Camera.main != null) {
			clickSound = RCC_CreateAudioSource.NewAudioSource (Camera.main.gameObject, HR_HighwayRacerProperties.Instance.buttonClickAudioClip.name, 0f, 0f, 1f, HR_HighwayRacerProperties.Instance.buttonClickAudioClip, false, true, true);
			clickSound.ignoreListenerPause = true;
			clickSound.ignoreListenerVolume = true;
		}

	}

}
