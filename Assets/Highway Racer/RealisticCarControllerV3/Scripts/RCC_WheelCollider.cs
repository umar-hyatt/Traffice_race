//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------


using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Based on Unity's WheelCollider. Modifies few curves, settings in order to get stable and realistic physics depends on selected behavior in RCC Settings.
/// </summary>
[RequireComponent (typeof(WheelCollider))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Wheel Collider")]
public class RCC_WheelCollider : MonoBehaviour {

	// Getting an Instance of Main Shared RCC Settings.
	#region RCC Settings Instance

	private RCC_Settings RCCSettingsInstance;
	private RCC_Settings RCCSettings {
		get {
			if (RCCSettingsInstance == null) {
				RCCSettingsInstance = RCC_Settings.Instance;
			}
			return RCCSettingsInstance;
		}
	}

	#endregion

	// Getting an Instance of Ground Materials.
	#region RCC_GroundMaterials Instance

	private RCC_GroundMaterials RCCGroundMaterialsInstance;
	private RCC_GroundMaterials RCCGroundMaterials {
		get {
			if (RCCGroundMaterialsInstance == null) {
				RCCGroundMaterialsInstance = RCC_GroundMaterials.Instance;
			}
			return RCCGroundMaterialsInstance;
		}
	}

	#endregion

	private WheelCollider _wheelCollider;
	public WheelCollider wheelCollider{
		get{
			if(_wheelCollider == null)
				_wheelCollider = GetComponent<WheelCollider>();
			return _wheelCollider;
		}
	}
	
	private RCC_CarControllerV3 carController;
	private Rigidbody rigid;

	private List <RCC_WheelCollider> allWheelColliders = new List<RCC_WheelCollider>() ;		// All other wheelcolliders attached to this vehicle.
	public Transform wheelModel;		// Wheel model.

	private float wheelRotation = 0f;		// Wheel model rotation based on WheelCollider rpm. 
	public float camber = 0f;					// Camber angle.

	internal float wheelRPMToSpeed = 0f;		// Wheel RPM to Speed.

	private RCC_GroundMaterials physicsMaterials{get{return RCCGroundMaterials;}}		// Instance of Configurable Ground Materials.
	private RCC_GroundMaterials.GroundMaterialFrictions[] physicsFrictions{get{return RCCGroundMaterials.frictions;}}

	private RCC_Skidmarks skidmarks;		// Main Skidmarks Manager class.
	private float startSlipValue = .25f;		// Draw skidmarks when forward or sideways slip is bigger than this value.
	private int lastSkidmark = -1;

	private float wheelSlipAmountForward = 0f;		// Forward slip.
	private float wheelSlipAmountSideways = 0f;	// Sideways slip.
	internal float totalSlip = 0f;		// Total amount of forward and sideways slips.

	//WheelFriction Curves and Stiffness.
	public WheelFrictionCurve forwardFrictionCurve;
	public WheelFrictionCurve sidewaysFrictionCurve;

	private AudioSource audioSource;		// Audiosource for tire skid SFX.
	private AudioClip audioClip;		// Audioclip for tire skid SFX.

	// List for all particle systems.
	internal List<ParticleSystem> allWheelParticles = new List<ParticleSystem>();
	internal ParticleSystem.EmissionModule emission;

	internal float tractionHelpedSidewaysStiffness = 1f;

	private float minForwardStiffness = .75f;
	private float maxForwardStiffness  = 1f;

	private float minSidewaysStiffness = .75f;
	private float maxSidewaysStiffness = 1f;

	TerrainData mTerrainData;
	int alphamapWidth;
	int alphamapHeight;

	float[,,] mSplatmapData;
	float mNumTextures;

	public float compression;
	
	void Awake (){
		
		carController = GetComponentInParent<RCC_CarControllerV3>();
		rigid = carController.GetComponent<Rigidbody> ();

		// Getting all WheelColliders attached to this vehicle (Except this).
		allWheelColliders = GetComponentsInChildren<RCC_WheelCollider>().ToList();
		allWheelColliders.Remove(this);
		GetTerrainData ();

		// Are we going to use skidmarks? If we do, get or create SkidmarksManager on scene.
		if (!RCCSettings.dontUseSkidmarks) {
			
			if (GameObject.FindObjectOfType (typeof(RCC_Skidmarks)))
				skidmarks = GameObject.FindObjectOfType (typeof(RCC_Skidmarks)) as RCC_Skidmarks;
			 else
				skidmarks = (RCC_Skidmarks)Instantiate (RCCSettings.skidmarksManager, Vector3.zero, Quaternion.identity);

		}

		// Increasing WheelCollider mass for avoiding unstable behavior. Only in Unity 5.
		if(RCCSettings.useFixedWheelColliders)
			wheelCollider.mass = rigid.mass / 15f;

		// Getting friction curves.
		forwardFrictionCurve = wheelCollider.forwardFriction;
		sidewaysFrictionCurve = wheelCollider.sidewaysFriction;

		// Proper settings for selected behavior type.
		switch(RCCSettings.behaviorType){

		case RCC_Settings.BehaviorType.HighwayRacer:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, .25f, 2f, 2f, 2f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, .25f, 2f, 2f, 2f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, .15f, 1f);
			break;

		}

		// Assigning new frictons if one of the behavior preset selected above.
		wheelCollider.forwardFriction = forwardFrictionCurve;
		wheelCollider.sidewaysFriction = sidewaysFrictionCurve;

		// Creating audiosource for skid SFX.
		if(RCCSettings.useSharedAudioSources){
			
			if(!carController.transform.Find("All Audio Sources/Skid Sound AudioSource"))
				audioSource = RCC_CreateAudioSource.NewAudioSource(carController.gameObject, "Skid Sound AudioSource", 5, 50, 0, audioClip, true, true, false);
			else
				audioSource = carController.transform.Find("All Audio Sources/Skid Sound AudioSource").GetComponent<AudioSource>();
			
		}else{
			
			audioSource = RCC_CreateAudioSource.NewAudioSource(carController.gameObject, "Skid Sound AudioSource", 5, 50, 0, audioClip, true, true, false);
			audioSource.transform.position = transform.position;

		}

		// Creating all ground particles, and adding them to list.
		if (!RCCSettings.dontUseAnyParticleEffects) {

			for (int i = 0; i < RCCGroundMaterials.frictions.Length; i++) {

				GameObject ps = (GameObject)Instantiate (RCCGroundMaterials.frictions [i].groundParticles, transform.position, transform.rotation) as GameObject;
				emission = ps.GetComponent<ParticleSystem> ().emission;
				emission.enabled = false;
				ps.transform.SetParent (transform, false);
				ps.transform.localPosition = Vector3.zero;
				ps.transform.localRotation = Quaternion.identity;
				allWheelParticles.Add (ps.GetComponent<ParticleSystem> ());

			}

		}

	}

	void Update(){

		// Return if RCC is disabled.
		if (!carController.enabled)
			return;

		// Only runs when vehicle is active. Raycasts are used for WheelAlign().
		if(!carController.isSleeping)
			WheelAlign ();

	}
	
	void FixedUpdate (){

		// Return if RCC is disabled.
		if (!carController.enabled)
			return;

		wheelRPMToSpeed = (((wheelCollider.rpm * wheelCollider.radius) / 2.9f)) * rigid.transform.lossyScale.y;

		#region Motor Torque, TCS.

		//Applying WheelCollider Motor Torques Depends On Wheel Type Choice.
		switch (carController._wheelTypeChoise) {

		case RCC_CarControllerV3.WheelType.FWD:
			if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
				ApplyMotorTorque (carController.engineTorque);
			break;
		case RCC_CarControllerV3.WheelType.RWD:
			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
				ApplyMotorTorque (carController.engineTorque);
			break;
		case RCC_CarControllerV3.WheelType.AWD:
			ApplyMotorTorque (carController.engineTorque / 2f);
			break;
		case RCC_CarControllerV3.WheelType.BIASED:
			if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
				ApplyMotorTorque (((carController.engineTorque * (100 - carController.biasedWheelTorque)) / 100f) / 2f);
			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
				ApplyMotorTorque (((carController.engineTorque * carController.biasedWheelTorque) / 100f) / 2f);
			break;

		}

		if (carController.ExtraRearWheelsCollider.Length > 0 && carController.applyEngineTorqueToExtraRearWheelColliders) {

			for (int i = 0; i < carController.ExtraRearWheelsCollider.Length; i++) {

				if (this == carController.ExtraRearWheelsCollider [i])
					ApplyMotorTorque (carController.engineTorque);

			}

		}

		#endregion

		#region Steering.

		// Apply Steering if this wheel is one of the front wheels.
		if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
			ApplySteering ();

		#endregion

		#region Braking, ABS.

		// Apply Handbrake if this wheel is one of the rear wheels.
		if (carController._handbrakeInput > .1f) {

			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
				ApplyBrakeTorque ((carController.brakeTorque * 1.5f) * carController._handbrakeInput);

		} else {

			// Apply Braking to all wheels.
			if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
				ApplyBrakeTorque (carController.brakeTorque * (Mathf.Clamp (carController._brakeInput, 0f, 1f)));
			else
				ApplyBrakeTorque (carController.brakeTorque * (Mathf.Clamp (carController._brakeInput, 0f, 1f) / 2f));

		}

		#endregion

		#region ESP.

		// ESP System. All wheels have individual brakes. In case of loosing control of the vehicle, corresponding wheel will brake for gaining the control again.
		if (carController.ESP) {

			if (carController._handbrakeInput < .5f) {

				if (carController.underSteering) {

					if (this == carController.RearLeftWheelCollider)
						ApplyBrakeTorque ((carController.brakeTorque * carController.ESPStrength) * Mathf.Clamp (-carController.frontSlip, 0f, Mathf.Infinity));

					if (this == carController.RearRightWheelCollider)
						ApplyBrakeTorque ((carController.brakeTorque * carController.ESPStrength) * Mathf.Clamp (carController.frontSlip, 0f, Mathf.Infinity));

				}

				if (carController.overSteering) {

					if (this == carController.FrontLeftWheelCollider)
						ApplyBrakeTorque ((carController.brakeTorque * carController.ESPStrength) * Mathf.Clamp (-carController.rearSlip, 0f, Mathf.Infinity));

					if (this == carController.FrontRightWheelCollider)
						ApplyBrakeTorque ((carController.brakeTorque * carController.ESPStrength) * Mathf.Clamp (carController.rearSlip, 0f, Mathf.Infinity));

				}

			}

		}

		#endregion

		if (!carController.isSleeping){
			
			Frictions ();
			SkidMarks ();
			Audio ();

		}

	}

	// Aligning wheel model position and rotation.
	public void WheelAlign (){

		// Return if no wheel model selected.
		if(!wheelModel){
			
			Debug.LogError(transform.name + " wheel of the " + carController.transform.name + " is missing wheel model. This wheel is disabled");
			enabled = false;
			return;

		}

		WheelHit GroundHit;
		bool grounded = wheelCollider.GetGroundHit(out GroundHit );

		float newCompression = compression;

		//Check if the wheel is grounded and set the compresion value accordingly.
		if (grounded)
			newCompression = 1f - ((Vector3.Dot(wheelCollider.transform.position - GroundHit.point, wheelCollider.transform.up) - (wheelCollider.radius * transform.lossyScale.y)) / wheelCollider.suspensionDistance);
		else
			newCompression = wheelCollider.suspensionDistance;

		compression = Mathf.Lerp (compression, newCompression, Time.deltaTime * 50f);

		// Set the position of the wheel model.
		wheelModel.position = new Vector3(wheelCollider.transform.position.x, (wheelCollider.transform.position.y + (compression - 1.0f) * wheelCollider.suspensionDistance), wheelCollider.transform.position.z);  

		// X axis rotation of the wheel.
		wheelRotation += wheelCollider.rpm * 6f * Time.deltaTime;
		wheelModel.transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(wheelRotation, wheelCollider.steerAngle, wheelCollider.transform.rotation.z);

		// Z axis rotation of the wheel for camber.
		if(wheelCollider.transform.localPosition.x > 0f)
			wheelCollider.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis((camber), Vector3.forward);
		else
			wheelCollider.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis((-camber), Vector3.forward);

		// Gizmos for wheel forces and slips.
		float extension = (-wheelCollider.transform.InverseTransformPoint(GroundHit.point).y - (wheelCollider.radius * transform.lossyScale.y)) / wheelCollider.suspensionDistance;
		Debug.DrawLine(GroundHit.point, GroundHit.point + wheelCollider.transform.up * (GroundHit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
		Debug.DrawLine(GroundHit.point, GroundHit.point - wheelCollider.transform.forward * GroundHit.forwardSlip * 2f, Color.green);
		Debug.DrawLine(GroundHit.point, GroundHit.point - wheelCollider.transform.right * GroundHit.sidewaysSlip * 2f, Color.red);

	}

	// Creating skidmarks.
	void SkidMarks(){

		// First, we are getting groundhit data.
		WheelHit GroundHit;
		wheelCollider.GetGroundHit(out GroundHit);

		// Forward, sideways, and total slips.
		wheelSlipAmountSideways = Mathf.Abs(GroundHit.sidewaysSlip);
		wheelSlipAmountForward = Mathf.Abs(GroundHit.forwardSlip);
		totalSlip = (wheelSlipAmountSideways + wheelSlipAmountForward);

		// If scene has skidmarks manager...
		if(skidmarks){

			// If slips are bigger than target value...
			if (wheelSlipAmountSideways > startSlipValue || wheelSlipAmountForward > startSlipValue){

				Vector3 skidPoint = GroundHit.point + 2f * (rigid.velocity) * Time.deltaTime;

				if(rigid.velocity.magnitude > 1f){
					lastSkidmark = skidmarks.AddSkidMark(skidPoint, GroundHit.normal, (wheelSlipAmountSideways / 2f) + (wheelSlipAmountForward / 2f), lastSkidmark);
				}else{
					lastSkidmark = -1;
				}

			}else{
				
				lastSkidmark = -1;

			}

		}

	}

	// Setting ground frictions to wheel frictions.
	void Frictions(){

		int groundIndex = GetGroundMaterialIndex ();

		WheelHit hit;
		wheelCollider.GetGroundHit (out hit);

		float hbInput = carController._handbrakeInput;

		if ((this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider) && hbInput > .7f)
			hbInput = .7f;
		else
			hbInput = 1;

		// Setting wheel stiffness to ground physic material stiffness.
		forwardFrictionCurve.stiffness = physicsFrictions[groundIndex].forwardStiffness;
		sidewaysFrictionCurve.stiffness = (physicsFrictions[groundIndex].sidewaysStiffness * hbInput * tractionHelpedSidewaysStiffness);

		// Setting new friction curves to wheels.
		wheelCollider.forwardFriction = forwardFrictionCurve;
		wheelCollider.sidewaysFriction = sidewaysFrictionCurve;

		// Also damp too.
		wheelCollider.wheelDampingRate = physicsFrictions[groundIndex].damp;

		// Set audioclip to ground physic material sound.
		audioClip = physicsFrictions[groundIndex].groundSound;

		// If wheel slip is bigger than ground physic material slip, enable particles. Otherwise, disable particles.
		if (!RCCSettings.dontUseAnyParticleEffects) {

			for (int i = 0; i < allWheelParticles.Count; i++) {

				if (wheelSlipAmountSideways > physicsFrictions [groundIndex].slip || wheelSlipAmountForward > physicsFrictions [groundIndex].slip) {

					if (i != groundIndex) {

						ParticleSystem.EmissionModule em;

						em = allWheelParticles [i].emission;
						em.enabled = false;

					} else {

						ParticleSystem.EmissionModule em;

						em = allWheelParticles [i].emission;
						em.enabled = true;

					}

				} else {

					ParticleSystem.EmissionModule em;

					em = allWheelParticles [i].emission;
					em.enabled = false;

				}

			}

		}

	}

	void Drift(){
		
		Vector3 relativeVelocity = transform.InverseTransformDirection(rigid.velocity);
		Vector3 relativeAngularVelocity = transform.InverseTransformDirection(rigid.angularVelocity);

//		float sqrVel = ((relativeVelocity.x * relativeVelocity.x) + (relativeAngularVelocity.y * relativeAngularVelocity.y)) / 50f;
		float sqrVel = ((relativeVelocity.x * relativeVelocity.x) + (relativeAngularVelocity.y * relativeAngularVelocity.y)) / 100f;

		// Forward
		if(wheelCollider == carController.FrontLeftWheelCollider.wheelCollider || wheelCollider == carController.FrontRightWheelCollider.wheelCollider){
			forwardFrictionCurve.extremumValue = Mathf.Clamp(1f - sqrVel, .5f, maxForwardStiffness);
			forwardFrictionCurve.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .5f, minForwardStiffness);
		}else{
			forwardFrictionCurve.extremumValue = Mathf.Clamp(1f - sqrVel, 1f, maxForwardStiffness);
			forwardFrictionCurve.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), 1.5f, minForwardStiffness);
		}

		// Sideways
		if(wheelCollider == carController.FrontLeftWheelCollider.wheelCollider || wheelCollider == carController.FrontRightWheelCollider.wheelCollider){
			sidewaysFrictionCurve.extremumValue = Mathf.Clamp(1f - sqrVel, .45f, maxSidewaysStiffness);
			sidewaysFrictionCurve.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .45f, minSidewaysStiffness);
		}else{
			sidewaysFrictionCurve.extremumValue = Mathf.Clamp(1f - sqrVel, .45f, maxSidewaysStiffness);
			sidewaysFrictionCurve.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .45f, minSidewaysStiffness);
		}

	}

	void Audio(){

		if(RCCSettings.useSharedAudioSources && isSkidding())
			return;
		
		if(totalSlip > startSlipValue){

			if(audioSource.clip != audioClip)
				audioSource.clip = audioClip;

			if(!audioSource.isPlaying)
				audioSource.Play();

			if(rigid.velocity.magnitude > 1f){
				
				audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Lerp(0f, 1f, totalSlip - startSlipValue), Time.fixedDeltaTime * 5f);
				audioSource.pitch = Mathf.Lerp(1f, .8f, audioSource.volume);

			}else{
				
				audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.fixedDeltaTime * 5f);

			}
			
		}else{
			
			audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.fixedDeltaTime * 5f);

			if(audioSource.volume <= .05f && audioSource.isPlaying)
				audioSource.Stop();
			
		}

	}

	bool isSkidding(){

		for (int i = 0; i < allWheelColliders.Count; i++) {

			if(allWheelColliders[i].totalSlip > totalSlip)
				return true;

		}

		return false;

	}

	void ApplyMotorTorque(float torque){

		if(carController.TCS){

			WheelHit hit;
			wheelCollider.GetGroundHit(out hit);

			if(Mathf.Abs(wheelCollider.rpm) >= 100){
				
				if(hit.forwardSlip > .25f){
					
					carController.TCSAct = true;
					torque -= Mathf.Clamp(torque * (hit.forwardSlip) * carController.TCSStrength, 0f, carController.engineTorque);

				}else{
					
					carController.TCSAct = false;
					torque += Mathf.Clamp(torque * (hit.forwardSlip) * carController.TCSStrength, -carController.engineTorque, 0f);

				}

			}else{
				
				carController.TCSAct = false;

			}

		}

		if(OverTorque())
			torque = 0;

		wheelCollider.motorTorque = ((torque * (1 - carController.clutchInput) * carController._boostInput) * carController._gasInput) * (carController.engineTorqueCurve[carController.currentGear].Evaluate(wheelRPMToSpeed * carController.direction) * carController.direction);

	}

	public void ApplySteering(){

		wheelCollider.steerAngle = carController.steerAngle * (carController._steerInput);

		wheelCollider.steerAngle += 20f * carController._verticalAngle;

		if (carController.roadPooling) {

			if (carController.transform.position.x > carController.roadPooling.roadWidth)
				wheelCollider.steerAngle = Mathf.Clamp (wheelCollider.steerAngle, -carController.steerAngle, 0f) + (carController.roadAngle * 20f);
			else if (carController.transform.position.x < -carController.roadPooling.roadWidth)
				wheelCollider.steerAngle = Mathf.Clamp (wheelCollider.steerAngle, 0f, carController.steerAngle) + (carController.roadAngle * 20f);

		}

	}

	void ApplyBrakeTorque(float brake){

		if(carController.ABS && carController._handbrakeInput <= .1f){

			WheelHit hit;
			wheelCollider.GetGroundHit(out hit);

			if((Mathf.Abs(hit.forwardSlip) * Mathf.Clamp01(brake)) >= carController.ABSThreshold){
				carController.ABSAct = true;
				brake = 0;
			}else{
				carController.ABSAct = false;
			}

		}

		wheelCollider.brakeTorque = brake;

	}

	bool OverTorque(){

		if(carController.speed > carController.maxspeed || !carController.engineRunning)
			return true;

		return false;

	}

	void GetTerrainData(){

		if (!Terrain.activeTerrain)
			return;
		
		mTerrainData = Terrain.activeTerrain.terrainData;
		alphamapWidth = mTerrainData.alphamapWidth;
		alphamapHeight = mTerrainData.alphamapHeight;

		mSplatmapData = mTerrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
		mNumTextures = mSplatmapData.Length / (alphamapWidth * alphamapHeight);

	}

	private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos){
		
		Vector3 vecRet = new Vector3();
		Terrain ter = Terrain.activeTerrain;
		Vector3 terPosition = ter.transform.position;
		vecRet.x = ((playerPos.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
		vecRet.z = ((playerPos.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
		return vecRet;

	}

	int GetGroundMaterialIndex(){

		// Contacted any physic material in Configurable Ground Materials yet?
		bool contacted = false;

		// First, we are getting groundhit data.
		WheelHit GroundHit;
		wheelCollider.GetGroundHit(out GroundHit);

		if (GroundHit.point == Vector3.zero)
			return 0;

		int ret = 0;
		
		for (int i = 0; i < physicsFrictions.Length; i++) {

			if (GroundHit.collider.sharedMaterial == physicsFrictions [i].groundMaterial) {
				
				contacted = true;
				ret = i;

			}

		}

		// If ground pyhsic material is not one of the ground material in Configurable Ground Materials, check if we are on terrain collider...
		if(!contacted && physicsMaterials.useTerrainSplatMapForGroundFrictions && GroundHit.collider.sharedMaterial == RCCGroundMaterials.terrainPhysicMaterial){

			Vector3 playerPos = transform.position;
			Vector3 TerrainCord = ConvertToSplatMapCoordinate(playerPos);
			float comp = 0f;

			for (int i = 0; i < mNumTextures; i++){
				
				if (comp < mSplatmapData[(int)TerrainCord.z, (int)TerrainCord.x, i])
					ret = i;
				
			}

		}

		return ret;

	}

	// Setting a new friction to WheelCollider.
	public WheelFrictionCurve SetFrictionCurves(WheelFrictionCurve curve, float extremumSlip, float extremumValue, float asymptoteSlip, float asymptoteValue){

		WheelFrictionCurve newCurve = curve;

		newCurve.extremumSlip = extremumSlip;
		newCurve.extremumValue = extremumValue;
		newCurve.asymptoteSlip = asymptoteSlip;
		newCurve.asymptoteValue = asymptoteValue;

		return newCurve;

	}

}