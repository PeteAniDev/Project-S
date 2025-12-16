using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class QueueObject : WorldObject {

	public string objName;
	public int queueTime = 0;
	public bool endTurn = false;
	public int turnCost = 0;

	public virtual void BeforeTurn() {
	}

	public virtual int AfterTurn() {
		endTurn = false;
		int c = turnCost;
		turnCost = 0;
		return c;
	}

	public virtual bool WhileTurn() {
		return endTurn;
	}

}
