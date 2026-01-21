using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.GridLayoutGroup;

public class Burn : StatusEffect {

	public override void ApplyEffect(Entity target, int level) {
		source.Damage(target, level);
	}

	public static void Apply(Entity source, Entity target, int level) {
		Burn burn = target.gameObject.GetComponent<Burn>();
		if (burn == null) {
			burn = target.gameObject.AddComponent<Burn>();
		}
		burn.source = source;
		burn.target = target;
		burn.level += level;
	}

}
