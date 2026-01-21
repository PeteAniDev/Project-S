using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Disable : CrowdControlEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canAttack = false;
	}

}
