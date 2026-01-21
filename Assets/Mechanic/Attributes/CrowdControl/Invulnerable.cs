using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Invulnerable : CrowdControlEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.isVulnerable = false;
	}

}
