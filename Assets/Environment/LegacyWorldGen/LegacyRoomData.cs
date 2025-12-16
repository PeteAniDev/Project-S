using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LegacyRoomData {

	public Vector2Int a;
	public Vector2Int b;
	public List<LegacyRoomConnection> connections = new List<LegacyRoomConnection>();

	public LegacyRoomData(int ax, int ay, int bx, int by) {
		a = new Vector2Int(Mathf.Min(ax, bx), Mathf.Min(ay, by));
		b = new Vector2Int(Mathf.Max(ax, bx), Mathf.Max(ay, by));
	}

	public Vector2 Center() {
		return (Vector2)(a + b) / 2;
	}

}
