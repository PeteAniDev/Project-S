using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SelfHealSpell : Spell {

	public int healing = 10;

	public SelfHealSpell() : base(50) {
	}

	public override void OnCast(Vector2Int position) {
		base.OnCast(position);
		owner.Heal(owner, healing);
	}

}
