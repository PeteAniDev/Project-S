using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

public class Stun : CrowdControlEffect {

	public override void ApplyEffect(StatusAttributes target) {
		target.canAttack = false;
		target.canCast = false;
		target.canMove = false;
	}

	public static void Apply(StatusAttributes target, int duration) {
		if (target == null) {
			return;
		}
		Stun stun = target.gameObject.AddComponent<Stun>();
		stun.duration = duration;
		stun.ApplyEffect(target);
	}

}
