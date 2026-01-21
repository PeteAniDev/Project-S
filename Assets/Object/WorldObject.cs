using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldObject : GridLock {

	public static List<WorldObject> objects = new List<WorldObject>();
	public static Dictionary<Vector2Int, List<WorldObject>> objectMap = new Dictionary<Vector2Int, List<WorldObject>>();

	public Hitbox hitbox;
	public bool movable = false;

	public virtual void OnEnable() {
		objects.Add(this);
		if (!objectMap.ContainsKey(gridPosition)) {
			objectMap[gridPosition] = new List<WorldObject>();
		}
		objectMap[gridPosition].Add(this);
	}

	public virtual void OnDisable() {
		objects.Remove(this);
		if (objectMap.ContainsKey(gridPosition)) {
			if (objectMap[gridPosition].Contains(this)) {
				objectMap[gridPosition].Remove(this);
			}
		}
	}

	public override void OnPositionChange() {
		if (objectMap.ContainsKey(lastPosition)) {
			objectMap[lastPosition].Remove(this);
			if (objectMap[lastPosition].Count == 0) {
				objectMap.Remove(lastPosition);
			}
		}
		if (!objectMap.ContainsKey(gridPosition)) {
			objectMap[gridPosition] = new List<WorldObject>();
		}
		if (objectMap[gridPosition].Contains(this)) {
			return;
		}
		objectMap[gridPosition].Add(this);
	}

}
