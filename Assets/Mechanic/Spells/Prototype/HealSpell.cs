using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HealSpell : Spell {

	public int healing = 10;

	public HealSpell() : base(70) {
	}

	public override void OnEntityHitOnce(Entity entity) {
		if (entity.IsTeam(owner)) {
			owner.Heal(entity, healing);
		}
	}

}
