using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Player : Entity {

	protected override void Start() {
		base.Start();
		hitbox = new Hitbox(new List<Vector2Int>() { Vector2Int.zero });
	}

	public override bool IsTeam(Entity owner) {
		if (owner.CompareTag("Player")) {
			return true;
		}
		return false;
	}

	public override bool WhileTurn() {
		return endTurn;
	}

}
