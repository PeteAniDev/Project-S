using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spell {

	public Hitbox hitbox;
	public Entity owner;

	public virtual void Cast(Vector2Int position) {
	}

	public virtual void OnEntityHit(Entity entity, Vector2Int contactPosition) {
	}

	public virtual void OnObjectHit(WorldObject obj, Vector2Int contactPosition) {
	}

}
