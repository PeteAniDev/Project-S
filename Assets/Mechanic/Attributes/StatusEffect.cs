using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class StatusEffect : MonoBehaviour {

	public int level = 0;
	public int levelReduction = 1;
	public Entity source;
	public Entity target;

	public abstract void ApplyEffect(Entity target, int level);

	public virtual void OnEnable() {
		SystemTurn.instance.systemQueue.Add(OnTurn);
	}

	public virtual void OnTurn() {
		ApplyEffect(target, level);

		level -= levelReduction;
		if (level <= 0) {
			Destroy(this);
		} else {
			SystemTurn.instance.systemQueue.Add(OnTurn);
		}
	}

}
