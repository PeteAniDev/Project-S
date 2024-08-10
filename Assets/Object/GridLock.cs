using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GridLock : MonoBehaviour {

	[System.NonSerialized]
	public Vector2Int gridPosition;
	public Vector2 gridOffset = Vector2.zero;
	public float heightOffset = 0;
	public bool gridSnap = true;

	private void Start() {
		gridPosition = ToGridPos(transform.position);
	}

	private void Update() {
		if (gridSnap) {
			transform.position = ToWorldPos(gridPosition + gridOffset) + new Vector2(0, heightOffset);
		} else {
			gridPosition = ToGridPos(transform.position - new Vector3(0, heightOffset, 0));
		}
	}

	public static Vector2 ToWorldPos(Vector2 pos) {
		return new Vector2(pos.x, -pos.x * 0.5f) + new Vector2(pos.y, pos.y * 0.5f);
	}

	public static Vector2Int ToGridPos(Vector2 pos) {
		float y = (pos.x + pos.y / 0.5f) / 2;
		float x = pos.x - y;
		return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
	}

}
