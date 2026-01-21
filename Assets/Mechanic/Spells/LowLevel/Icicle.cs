using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class Icicle : Spell {

	private const int BASE_DAMAGE = 10;
	private const DamageType DAMAGE_TYPE = DamageType.Ice;

	public Icicle() : base(100) {
		castbox = Hitbox.NewSquare(4);
		hitbox = Hitbox.NewSquare(1);
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
		// Slow? Idk
	}

}
