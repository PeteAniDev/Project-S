using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class ActiveObject : QueueObject {

	public static List<ActiveObject> objects = new List<ActiveObject>();

	public Hitbox hitbox;
	public bool movable = false;

	private void OnEnable() {
		objects.Add(this);
	}

	private void OnDisable() {
		objects.Remove(this);
	}

}
