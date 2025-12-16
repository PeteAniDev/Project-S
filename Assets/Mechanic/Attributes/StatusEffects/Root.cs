using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Root : StatusEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canMove = false;
	}

}
