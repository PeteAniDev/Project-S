using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class Interactable : WorldObject {

	public bool Interact(Entity interacter) {
		if (CanInteract(interacter)) {
			Interaction(interacter);
			return true;
		}
		return false;
	}

	public abstract bool CanInteract(Entity interacter);

	public abstract void Interaction(Entity interacter);

	public bool Overlap(Entity interacter) {
		return hitbox.Overlap(interacter.hitbox);
	}

}
