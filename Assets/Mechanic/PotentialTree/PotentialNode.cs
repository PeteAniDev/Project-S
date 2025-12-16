using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public enum PotentialType {
	Blue,
	Purple,
	Gold
}

public class PotentialNode : MonoBehaviour {

	public List<PotentialConnection> connections = new List<PotentialConnection>();

	public bool unlocked = false;
	public PotentialType type = PotentialType.Blue;
	public int cost = 10;

	public void Connect(PotentialNode to, PotentialConnectionType type) {
		PotentialConnection connection = new PotentialConnection(this, to, type);
		connections.Add(connection);
		to.connections.Add(connection);
	}

	public bool CanUnlock(PotentialType type, int current) {
		return type == this.type && current >= cost;
	}

	public bool TryUnlock() {
		bool canUnlock = false;
		foreach (PotentialConnection connection in connections) {
			if (connection.type == PotentialConnectionType.Restrict) {
				if (connection.GetOther(this).unlocked) {
					return false;
				}
			}
			if (connection.type == PotentialConnectionType.BothWays) {
				if (connection.GetOther(this).unlocked) {
					canUnlock = true;
				}
			}
			if (connection.type == PotentialConnectionType.Directional) {
				if (connection.to == this && connection.from.unlocked) {
					canUnlock = true;
				}
			}
		}
		if (canUnlock) {
			unlocked = true;
			return true;
		} else {
			return false;
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = unlocked ? Color.white : Color.gray;
		switch (type) {
			case PotentialType.Blue:
				Gizmos.color *= Color.cyan;
				DrawCircleGizmo(transform.position, Vector3.forward, 0.25f, 16);
				break;
			case PotentialType.Purple:
				Gizmos.color *= Color.magenta;
				DrawRectGizmo(transform.position, 0.25f);
				break;
			case PotentialType.Gold:
				Gizmos.color *= Color.yellow;
				DrawDiamondGizmo(transform.position, 0.25f);
				break;
		}
	}

	public void DrawCircleGizmo(Vector3 center, Vector3 normal, float radius, int segments) {
		if (radius <= 0 || segments <= 0) {
			return; // Invalid input
		}

		// Calculate the initial forward and right vectors for the circle's plane
		Vector3 forward = (normal.x < normal.z) ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 0f, 1f);
		forward = Vector3.Cross(normal, forward).normalized;
		Vector3 right = Vector3.Cross(forward, normal).normalized;

		Vector3 previousPoint = center + (forward * radius);
		float angleStep = (Mathf.PI * 2f) / segments;

		for (int i = 0; i < segments; i++) {
			float angle = (i + 1) * angleStep;
			// Ensure the last segment connects back to the start for a closed loop
			if (i == segments - 1) {
				angle = 0f;
			}

			Vector3 nextPointLocal = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;
			Vector3 nextPoint = center + (right * nextPointLocal.x) + (forward * nextPointLocal.z);

			Gizmos.DrawLine(previousPoint, nextPoint);
			previousPoint = nextPoint;
		}
	}

	public void DrawRectGizmo(Vector3 center, float extend) {
		Vector3[] points = new Vector3[] { new Vector3(extend, extend, 0), new Vector3(extend, -extend, 0), new Vector3(-extend, -extend, 0), new Vector3(-extend, extend, 0) };
		for (int i = 0; i < points.Length; i++) {
			int nextIndex = (i + 1) % points.Length;
			Gizmos.DrawLine(center + points[i], center + points[nextIndex]);
		}
	}

	public void DrawDiamondGizmo(Vector3 center, float extend) {
		Vector3[] points = new Vector3[] { new Vector3(extend, 0, 0), new Vector3(0, -extend, 0), new Vector3(-extend, 0, 0), new Vector3(0, extend, 0) };
		for (int i = 0; i < points.Length; i++) {
			int nextIndex = (i + 1) % points.Length;
			Gizmos.DrawLine(center + points[i], center + points[nextIndex]);
		}
	}

}
