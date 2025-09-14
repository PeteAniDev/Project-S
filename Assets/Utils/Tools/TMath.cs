using System.Collections.Generic;

using UnityEngine;

public class TMath {

	public static bool RandBool() {
		return Random.value > 0.5;
	}

	public static int RandInt(int min, int max) {
		return Random.Range(min, max + 1);
	}

	public static float Angle(Vector2 from, Vector2 to) {
		return -Vector2.SignedAngle(from, to);
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

	public static T PickRandom<T>(List<T> list) {
		return list[Random.Range(0, list.Count)];
	}

	public static int GetRandomOdd(int[] odds) {
		int total = 0;
		for (int i = 0; i < odds.Length; i++) {
			if (odds[i] > 0) {
				total += odds[i];
			}
		}
		int selection = Random.Range(0, total);
		int current = 0;
		for (int i = 0; i < odds.Length; i++) {
			if (odds[i] > 0) {
				current += odds[i];
			}
			if (selection < current) {
				return i;
			}
		}
		return Random.Range(0, odds.Length);
	}

	public static int GetRandomOdd(System.Random rand, int[] odds) {
		int total = 0;
		for (int i = 0; i < odds.Length; i++) {
			if (odds[i] > 0) {
				total += odds[i];
			}
		}
		int selection = rand.Next(0, total);
		int current = 0;
		for (int i = 0; i < odds.Length; i++) {
			if (odds[i] > 0) {
				current += odds[i];
			}
			if (selection < current) {
				return i;
			}
		}
		return rand.Next(0, odds.Length);
	}

}
