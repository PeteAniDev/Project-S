using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class ArcaneBolt : Spell {

	private const int BASE_DAMAGE = 20;
	private const DamageType DAMAGE_TYPE = DamageType.Arcane;
	private const int RANGE = 5;

	private Vector2Int castDir;

	public ArcaneBolt() : base(20) {
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
		owner.Damage(entity, BASE_DAMAGE);
		hitbox.extends.Clear();
	}

}
