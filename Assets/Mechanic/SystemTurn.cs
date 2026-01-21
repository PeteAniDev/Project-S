using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SystemTurn : QueueObject {

	public static SystemTurn instance;

	public List<Action> systemQueue = new List<Action>();

	private List<Action> queued = new List<Action>();

	private void Awake() {
		instance = this;
	}

	public override void OnEnable() {
	}

	public override void OnDisable() {
	}

	public override void OnPositionChange() {
	}

	public override void BeforeTurn() {
		endTurn = true;
		queued.AddRange(systemQueue);
		systemQueue.Clear();
		foreach (Action action in queued) {
			action();
		}
	}

	public override int AfterTurn() {
		endTurn = false;
		queued.Clear();
		return turnCost;
	}

	public override bool WhileTurn() {
		return endTurn;
	}

}
