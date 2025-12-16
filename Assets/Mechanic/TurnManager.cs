using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

public class TurnManager : MonoBehaviour {

	public static TurnManager instance;

	public List<QueueObject> queue = new List<QueueObject>();

	public QueueObject currentTurn = null;

	private void Awake() {
		instance = this;
	}

	private void Update() {
		if (currentTurn == null) {
			if (queue.Count <= 0) {
				return;
			}
			currentTurn = queue[0];
			currentTurn.BeforeTurn();
		}

		if (currentTurn.WhileTurn()) {
			Queue(currentTurn, currentTurn.AfterTurn());
			currentTurn = null;
		}
	}

	public void Queue(QueueObject obj, int queueTime) {
		obj.queueTime += queueTime;
		if (queue.Contains(obj)) {
			queue.Remove(obj);
		}
		for (int i = 0; i < queue.Count; i++) {
			if (queue[i].queueTime > obj.queueTime) {
				queue.Insert(i, obj);
				return;
			}
		}
		queue.Add(obj);
	}

	public void DropQueue(QueueObject obj) {
		if (queue.Contains(obj)) {
			queue.Remove(obj);
		}
	}

	public void ClearQueue() {
		queue.Clear();
	}

	private void OnDrawGizmos() {
		if (queue == null || queue.Count == 0 || Camera.main == null) {
			return;
		}

		string queueText = string.Join(" -> ", queue.Select(obj => $"{obj.objName}({obj.queueTime})"));

		Vector3 screenCenterTop = new Vector3(Screen.width / 2, Screen.height - 20);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenCenterTop);

		Handles.color = Color.black;
		Handles.Label(worldPos, queueText);
	}

}
