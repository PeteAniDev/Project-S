using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public enum PotentialConnectionType {
	BothWays = 0,
	Restrict = 1,
	Directional = 2
}

[Serializable]
public struct PotentialConnection {

	public PotentialNode from;
	public PotentialNode to;
	public PotentialConnectionType type;

	public PotentialConnection(PotentialNode from, PotentialNode to, PotentialConnectionType type) {
		this.from = from;
		this.to = to;
		this.type = type;
	}

	public PotentialNode GetOther(PotentialNode node) {
		if (node == from) {
			return to;
		} else if (node == to) {
			return from;
		} else {
			return null;
		}
	}

	public void Apply() {
		if (!from.connections.Contains(this)) {
			from.connections.Add(this);
		}
		if (!to.connections.Contains(this)) {
			to.connections.Add(this);
		}
	}

}
