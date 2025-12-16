using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Disable : StatusEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canAttack = false;
	}

}
