using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class WaterSplash : Spell {

	private const int BASE_DAMAGE = 10;
	private const DamageType DAMAGE_TYPE = DamageType.Water;

	public WaterSplash() : base(100) {
		castbox = Hitbox.NewSquare(4);
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
	}

}
