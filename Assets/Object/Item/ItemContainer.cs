using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemContainer : MonoBehaviour {

	private List<Item> items = new List<Item>();

	public int capacity = 10;

	public bool CanAdd(Item item) {
		int currentWeight = 0;
		foreach (Item it in items) {
			currentWeight += it.weight;
		}
		return currentWeight + item.weight <= capacity;
	}

	public void ForceAdd(Item item) {
		items.Add(item);
	}

	public void TryAdd(Item item) {
		if (CanAdd(item)) {
			items.Add(item);
		}
	}

}
