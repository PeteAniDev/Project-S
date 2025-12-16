using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Invulnerable : StatusEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.isVulnerable = false;
	}

}
