using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Move : Spell {

	public Move() : base(25) {
		castbox = Hitbox.NewDiamond(2);
	}

	public override void OnCast(Vector2Int position) {
		base.OnCast(position);
		owner.gridPosition = position;
	}

	public override bool CanCast(Vector2Int castPosition, Vector2Int ownerPosition) {
		if (base.CanCast(castPosition, ownerPosition)) {
			if (WorldObject.objectMap.ContainsKey(castPosition)) {
				return WorldObject.objectMap[castPosition].Count == 0;
			}
			return true;
		}
		return false;
	}

}
