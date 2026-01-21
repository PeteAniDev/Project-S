using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class Fireball : Spell {

	private const int BASE_DAMAGE = 20;
	private const DamageType DAMAGE_TYPE = DamageType.Fire;
	private const int BASE_EFFECT = 4;
	private const int RANGE = 5;

	private Vector2Int castDir;

	public Fireball() : base(30) {
		castbox = Hitbox.NewCross(RANGE);
		hitbox.extends.Clear();
	}

	public override void OnCast(Vector2Int position) {
		castDir = (position - owner.gridPosition);
		castDir /= (int)castDir.magnitude;
		castPos = owner.gridPosition;
		hitbox.extends.Clear();
		for (int i = 1; i <= RANGE; i++) {
			hitbox.extends.Add(castDir * i);
		}
	}

	public override void OnEndCast(Vector2Int position) {
		hitbox.extends.Clear();
	}

	public override void OnEntityHitOnce(Entity entity) {
		if (owner.IsTeam(entity)) {
			return;
		}
		owner.Damage(entity, BASE_DAMAGE);
		hitbox.extends.Clear();
		Burn.Apply(owner, entity, BASE_EFFECT);
	}

}
