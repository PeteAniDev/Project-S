using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class IceShard : Spell {

	private const int BASE_DAMAGE = 10;
	private const DamageType DAMAGE_TYPE = DamageType.Ice;
	private const float BASE_FREEZE_RATE = 0.2f;

	public IceShard() : base(20) {
		castbox = Hitbox.NewSquare(4);
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
		if (Random.value < BASE_FREEZE_RATE) {
			// Apply Freeze
		}
	}

}
