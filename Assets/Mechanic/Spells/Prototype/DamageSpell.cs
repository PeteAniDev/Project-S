using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DamageSpell : Spell {

	public int damage = 15;

	public DamageSpell() : base(50) {
		hitbox = Hitbox.NewSquare(1);
		castbox = Hitbox.NewSquare(1);
	}

	public override void OnEntityHitOnce(Entity entity) {
		if (!owner.IsTeam(entity)) {
			owner.Damage(entity, damage);
		}
	}

}
