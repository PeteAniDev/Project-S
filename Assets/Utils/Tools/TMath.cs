using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TMath {

	public static bool RandBool() {
		return Random.value > 0.5;
	}

	public static int RandInt(int min, int max) {
		return Random.Range(min, max + 1);
	}

	public static Vector2 Rotate(Vector2 v, float degree) {
		float radian = Mathf.Deg2Rad * degree;
		return new Vector2(v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian), v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian));
	}

}
