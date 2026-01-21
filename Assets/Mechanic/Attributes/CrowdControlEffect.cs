using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class CrowdControlEffect : MonoBehaviour {

	public int duration = 0;

	public abstract void ApplyEffect(StatusAttributes target);

	public virtual void Calculate(int time) {
		duration -= time;
		if (duration <= 0) {
			Destroy(this);
		}
	}

}
