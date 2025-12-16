using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct Hitbox {

	public List<Vector2Int> extends;

	public Hitbox(List<Vector2Int> extends) {
		this.extends = extends;
	}

	public static Hitbox NewSquare(int size) {
		Hitbox hitbox = new Hitbox(new List<Vector2Int>());
		for (int x = -size; x <= size; x++) {
			for (int y = -size; y <= size; y++) {
				hitbox.extends.Add(new Vector2Int(x, y));
			}
		}
		return hitbox;
	}

	public static Hitbox NewDiamond(int size) {
		Hitbox hitbox = new Hitbox(new List<Vector2Int>());
		for (int x = -size; x <= size; x++) {
			for (int y = -size; y <= size; y++) {
				if (Mathf.Abs(x) + Mathf.Abs(y) <= size) {
					hitbox.extends.Add(new Vector2Int(x, y));
				}
			}
		}
		return hitbox;
	}

	public bool Overlap(Vector2Int vector2Int) {
		foreach (Vector2Int extend in extends) {
			if (extend == vector2Int) {
				return true;
			}
		}
		return false;
	}

	public bool Overlap(Hitbox hitbox) {
		foreach (Vector2Int extend in hitbox.extends) {
			return Overlap(extend);
		}
		return false;
	}

	public int OverlapCount(Hitbox hitbox) {
		int count = 0;
		foreach (Vector2Int extend in hitbox.extends) {
			if (Overlap(extend)) {
				count++;
			}
		}
		return count;
	}

}
