using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SocialPlatforms;

using static Element;

public class RockBullet : Spell {

	private const int BASE_DAMAGE = 10;
	private const DamageType DAMAGE_TYPE = DamageType.Earth;
	private const int RANGE = 4;

	private Vector2Int castDir;

	public RockBullet() : base(100) {
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
