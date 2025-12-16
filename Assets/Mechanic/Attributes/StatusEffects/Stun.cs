using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stun : StatusEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canAttack = false;
		target.canCast = false;
		target.canMove = false;
	}

}
