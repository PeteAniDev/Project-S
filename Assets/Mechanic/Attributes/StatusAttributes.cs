using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StatusAttributes : MonoBehaviour {

	public bool canMove = true;
	public bool canAttack = true;
	public bool canCast = true;
	public bool isVulnerable = true;

	// CC Resistance
	public float stunResistance = 0f;
	public float rootResistance = 0f;
	public float silenceResistance = 0f;
	public float disableResistance = 0f;
	public float fatigueResistance = 0f;
	public float purgeResistance = 0f;

	public void ResetAttributes() {
		canMove = true;
		canAttack = true;
		canCast = true;
		isVulnerable = true;
	}

}
