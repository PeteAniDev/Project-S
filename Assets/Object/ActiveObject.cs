using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class ActiveObject : QueueObject {

	public static List<ActiveObject> activeObjs = new List<ActiveObject>();

	public override void OnEnable() {
		base.OnEnable();
		activeObjs.Add(this);
	}

	public override void OnDisable() {
		activeObjs.Remove(this);
		base.OnDisable();
	}

}
