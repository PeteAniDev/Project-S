using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RoomData {

	public Vector2Int a;
	public Vector2Int b;

	public RoomData(int ax, int ay, int bx, int by) {
		a = new Vector2Int(ax, ay);
		b = new Vector2Int(bx, by);
	}

	public Vector2 Center() {
		return (Vector2)(a + b) / 2;
	}
}
