using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PushSpell : Spell {

	public PushSpell() : base(40) {
	}

	public override void OnEntityHit(Entity entity, Vector2Int contactPosition) {
		if (!entity.IsTeam(owner)) {
			return;
		}
		if (contactPosition.x > entity.gridPosition.x) {
			entity.Push(new Vector2Int(1, 0));
		} else if (contactPosition.x < entity.gridPosition.x) {
			entity.Push(new Vector2Int(-1, 0));
		} else if (contactPosition.y > entity.gridPosition.y) {
			entity.Push(new Vector2Int(0, 1));
		} else if (contactPosition.y < entity.gridPosition.y) {
			entity.Push(new Vector2Int(0, -1));
		}
	}

}
