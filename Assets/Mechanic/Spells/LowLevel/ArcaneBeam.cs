using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Element;

public class ArcaneBeam : Spell {

	private const int BASE_DAMAGE = 40;
	private const DamageType DAMAGE_TYPE = DamageType.Water;
	private const int RANGE = 5;

	private Vector2Int castDir;

	public ArcaneBeam() : base(35) {
		castTime = 10;
		castbox = Hitbox.NewCross(RANGE);
		hitbox.extends.Clear();
	}

	public override void OnCast(Vector2Int position) {
		castDir = (position - owner.gridPosition);
		castDir /= (int)castDir.magnitude;
		castPos = owner.gridPosition;
		hitbox.extends.Clear();
		// Add in reverse to ensure everyone on the line is correctly pushed
		for (int i = RANGE; i > 0; i--) {
			hitbox.extends.Add(castDir * i);
		}
	}

	public override void OnEndCast(Vector2Int position) {
		hitbox.extends.Clear();
	}

	public override void OnEntityHitOnce(Entity entity) {
		owner.Damage(entity, BASE_DAMAGE);
		entity.Push(castDir);
	}

}
