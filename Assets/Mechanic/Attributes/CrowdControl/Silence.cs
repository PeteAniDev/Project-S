using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Silence : CrowdControlEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canCast = false;
	}

}
