//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class HR_BarrierCollisionProtector : MonoBehaviour {

	public CollisionSide collisionSide;
	public enum CollisionSide{Left, Right}

	private Rigidbody playerRigid;

	void OnTriggerStay (Collider col) {

		if (!col.transform.CompareTag ("Player"))
			return;

		if(!playerRigid)
			playerRigid = col.gameObject.GetComponentInParent<RCC_CarControllerV3>().GetComponent<Rigidbody> ();

		if(collisionSide == CollisionSide.Right)
			playerRigid.AddForce (-Vector3.right * 100f, ForceMode.Acceleration);
		else
			playerRigid.AddForce (Vector3.right * 100f, ForceMode.Acceleration);

		playerRigid.velocity = new Vector3 (0f, playerRigid.velocity.y, playerRigid.velocity.z);
		playerRigid.angularVelocity =  new Vector3 (playerRigid.angularVelocity.x, 0f, 0f);
	
	}

	void OnDrawGizmos(){

		Gizmos.color = new Color(1f, .5f, 0f, .75f);
		Gizmos.DrawCube(transform.position, GetComponent<BoxCollider> ().size);

	}

}
