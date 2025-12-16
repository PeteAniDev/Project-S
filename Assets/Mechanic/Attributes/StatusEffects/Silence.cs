using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Silence : StatusEffect {

	public override void ApplyEffect(StatusAttributes target) {
		if (level > 0) {
			target.canCast = false;
		}
	}

}
