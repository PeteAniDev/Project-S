using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class PotentialTree : MonoBehaviour {

	public List<PotentialNode> nodes = new List<PotentialNode>();
	public List<PotentialConnection> connections = new List<PotentialConnection>();

	private void Start() {
		ApplyConnections();
	}

	public void ApplyConnections() {
		foreach (PotentialConnection connection in connections) {
			connection.Apply();
		}
	}

	private void OnDrawGizmos() {
		foreach (PotentialConnection connection in connections) {
			switch (connection.type) {
				case PotentialConnectionType.BothWays:
					Gizmos.color = Color.white;
					break;
				case PotentialConnectionType.Directional:
					Gizmos.color = Color.green;
					Gizmos.DrawSphere(connection.to.transform.position, 0.1f);
					break;
				case PotentialConnectionType.Restrict:
					Gizmos.color = Color.red;
					break;
			}
			Gizmos.DrawLine(connection.from.transform.position, connection.to.transform.position);
		}
	}

}
