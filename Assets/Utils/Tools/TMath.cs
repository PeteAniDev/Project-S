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

	public static bool CheckLSiLS(Vector2 a0, Vector2 b0, Vector2 a1, Vector2 b1) {
		bool softCheck = ((a0 + b0) / 2 - (a1 + b1) / 2).magnitude < ((a0 - b0).magnitude + (a1 - b1).magnitude) / 2;
		if (!softCheck) {
			return false;
		}
		return CheckLiLS(a0, a1, b0 - a0, b1 - a1) && CheckLiLS(a1, a0, b1 - a1, b0 - a0);
	}

	private static bool CheckLiLS(Vector2 p, Vector2 q, Vector2 r, Vector2 s) {
		double t = Cross(q - p, s) / Cross(r, s);
		return t > 0 && t < 1;
	}

	public static float Cross(Vector2 a, Vector2 b) {
		return a.x * b.y - a.y * b.x;
	}

}
