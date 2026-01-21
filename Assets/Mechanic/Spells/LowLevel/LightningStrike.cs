using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class LightningStrike : Spell {

	private const int BASE_DAMAGE = 20;
	private const DamageType DAMAGE_TYPE = DamageType.Lightning;

	public LightningStrike() : base(40) {
		castbox = Hitbox.NewSquare(4);
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
	}

}
