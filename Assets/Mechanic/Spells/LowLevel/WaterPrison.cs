using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class WaterPrison : Spell {

	private const int BASE_DAMAGE = 10;
	private const int BASE_CC = 40;
	private const DamageType DAMAGE_TYPE = DamageType.Water;

	public WaterPrison() : base(50) {
		castbox = Hitbox.NewSquare(4);
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
		// Apply Wet
		Stun.Apply(entity.GetComponent<StatusAttributes>(), BASE_CC);
	}

}
