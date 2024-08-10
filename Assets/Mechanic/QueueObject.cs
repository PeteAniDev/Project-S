using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class QueueObject : GridLock {

	public int queueTime = 0;

	public virtual void BeforeTurn() {
	}

	public virtual void AfterTurn() {
	}

	public abstract bool WhileTurn();

}
