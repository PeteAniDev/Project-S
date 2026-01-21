using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Item : Interactable {

	public int weight = 1;

	protected override void Start() {
		base.Start();
		hitbox = Hitbox.NewSquare(0);
	}

	public override bool CanInteract(Entity interacter) {
		if (!Overlap(interacter)) {
			return false;
		}

		ItemContainer container = interacter.GetComponent<ItemContainer>();
		if (container != null) {
			return container.CanAdd(this);
		}
		return false;
	}

	public void PickUp(ItemContainer container) {
		container.ForceAdd(this);
		gameObject.SetActive(false);
	}

	public override void Interaction(Entity interacter) {
		ItemContainer container = interacter.GetComponent<ItemContainer>();
		if (container != null) {
			PickUp(container);
		}
	}

}
