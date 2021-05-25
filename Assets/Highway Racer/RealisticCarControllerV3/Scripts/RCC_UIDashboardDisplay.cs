//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2015 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Dashboard Displayer")]
[RequireComponent (typeof(RCC_DashboardInputs))]
public class RCC_UIDashboardDisplay : MonoBehaviour {

	public DisplayType displayType;
	public enum DisplayType{Full, Customization, TopButtonsOnly, Off}

	public HR_PlayerHandler player;

	public Text score;
	public Text timeLeft;
	public Text combo;

	public Text speed;
	public Text distance;
	public Text highSpeed;
	public Text oppositeDirection;
	public Slider bombSlider;

	private Image comboMImage;
	private Vector2 comboDefPos;

	private Image highSpeedImage;
	private Vector2 highSpeedDefPos;

	private Image oppositeDirectionImage;
	private Vector2 oppositeDirectionDefPos;

	private Image timeAttackImage;

	private RectTransform bombRect;
	private Vector2 bombDefPos;

	void Awake () {

		comboMImage = combo.GetComponentInParent<Image>();
		comboDefPos = comboMImage.rectTransform.anchoredPosition;
		highSpeedImage = highSpeed.GetComponentInParent<Image>();
		highSpeedDefPos = highSpeedImage.rectTransform.anchoredPosition;
		oppositeDirectionImage = oppositeDirection.GetComponentInParent<Image>();
		oppositeDirectionDefPos = oppositeDirectionImage.rectTransform.anchoredPosition;
		timeAttackImage = timeLeft.GetComponentInParent<Image>();
		bombRect = bombSlider.GetComponent<RectTransform>();
		bombDefPos = bombRect.anchoredPosition;

		StartCoroutine("LateDisplay");

	}

	void HR_PlayerHandler_OnPlayerSpawned (HR_PlayerHandler _player){

		player = _player;
		
	}

	void OnEnable(){

		HR_PlayerHandler.OnPlayerSpawned += HR_PlayerHandler_OnPlayerSpawned;
		StopAllCoroutines();
		StartCoroutine("LateDisplay");

	}

	IEnumerator LateDisplay () {

		while(true){

			yield return new WaitForSeconds(.04f);

			score.text = player.score.ToString("F0");
			speed.text = player.speed.ToString("F0");
			distance.text = (player.distance).ToString("F2");
			highSpeed.text = player.highSpeedCurrent.ToString("F1");
			oppositeDirection.text = player.opposideDirectionCurrent.ToString("F1");
			timeLeft.text = player.timeLeft.ToString("F1");
			combo.text = player.combo.ToString();

			if(HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb)
				bombSlider.value = player.bombHealth / 100f;

		}

	}

	void Update(){

		if(!player)
			return;

		if(player.combo > 1){
			comboMImage.rectTransform.anchoredPosition = Vector2.Lerp(comboMImage.rectTransform.anchoredPosition, comboDefPos, Time.deltaTime * 5f);
		}else{
			comboMImage.rectTransform.anchoredPosition = Vector2.Lerp(comboMImage.rectTransform.anchoredPosition, new Vector2(comboDefPos.x - 500, comboDefPos.y), Time.deltaTime * 5f);
		}

		if(player.highSpeedCurrent > .1f){
			highSpeedImage.rectTransform.anchoredPosition = Vector2.Lerp(highSpeedImage.rectTransform.anchoredPosition, highSpeedDefPos, Time.deltaTime * 5f);
		}else{
			highSpeedImage.rectTransform.anchoredPosition = Vector2.Lerp(highSpeedImage.rectTransform.anchoredPosition, new Vector2(highSpeedDefPos.x + 500, highSpeedDefPos.y), Time.deltaTime * 5f);
		}

		if(player.opposideDirectionCurrent > .1f){
			oppositeDirectionImage.rectTransform.anchoredPosition = Vector2.Lerp(oppositeDirectionImage.rectTransform.anchoredPosition, oppositeDirectionDefPos, Time.deltaTime * 5f);
		}else{ 
			oppositeDirectionImage.rectTransform.anchoredPosition = Vector2.Lerp(oppositeDirectionImage.rectTransform.anchoredPosition, new Vector2(oppositeDirectionDefPos.x - 500, oppositeDirectionDefPos.y), Time.deltaTime * 5f);
		}

		if(HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.TimeAttack){
			if(!timeLeft.gameObject.activeSelf)
				timeAttackImage.gameObject.SetActive(true);
		}else{ 
			if(timeLeft.gameObject.activeSelf)
				timeAttackImage.gameObject.SetActive(false);
		}

		if(HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb){
			if(!bombSlider.gameObject.activeSelf)
				bombSlider.gameObject.SetActive(true);
		}else{ 
			if(bombSlider.gameObject.activeSelf)
				bombSlider.gameObject.SetActive(false);
		}

		if(player.bombTriggered){
			bombRect.anchoredPosition = Vector2.Lerp(bombRect.anchoredPosition, bombDefPos, Time.deltaTime * 5f);
		}else{
			bombRect.anchoredPosition = Vector2.Lerp(bombRect.anchoredPosition, new Vector2(bombDefPos.x - 500, bombDefPos.y), Time.deltaTime * 5f);
		}

	}

	public void SetDisplayType(DisplayType _displayType){

		displayType = _displayType;

	}

	void OnDisable(){

		HR_PlayerHandler.OnPlayerSpawned -= HR_PlayerHandler_OnPlayerSpawned;

	}

}
