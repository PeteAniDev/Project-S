using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spell {

	public Hitbox hitbox;
	public Hitbox castbox;
	public Entity owner;
	public Vector2Int castPos;
	public int castTime = 0;
	public int coolTime = 1;

	public Spell(int coolTime) {
		this.coolTime = coolTime;
		hitbox = new Hitbox(new List<Vector2Int>() { Vector2Int.zero });
		castbox = new Hitbox(new List<Vector2Int>() { Vector2Int.zero });
	}

	public virtual void RefreshCastBox() {
	}

	public void Cast(Vector2Int position) {
		OnCast(position);
		if (hitbox.extends != null) {
			for (int i = 0; i < hitbox.extends.Count; i++) {
				Vector2Int extend = hitbox.extends[i];
				foreach (Entity entity in Entity.entities) {
					bool hit = false;
					foreach (Vector2Int entityExtend in entity.hitbox.extends) {
						if (position + extend == entity.gridPosition + entityExtend) {
							OnEntityHit(entity, position + extend);
							hit = true;
						}
					}
					if (hit) {
						OnEntityHitOnce(entity);
					}
				}
				foreach (WorldObject obj in WorldObject.objects) {
					bool hit = false;
					foreach (Vector2Int objExtend in obj.hitbox.extends) {
						if (position + extend == obj.gridPosition + objExtend) {
							OnObjectHit(obj, position + extend);
							hit = true;
						}
					}
					if (hit) {
						OnObjectHitOnce(obj);
					}
				}
			}
		}
		OnEndCast(position);
		owner.endTurn = true;
		owner.turnCost += coolTime;
	}

	public virtual void OnCast(Vector2Int position) {
		castPos = position;
	}

	public virtual void OnEndCast(Vector2Int position) {
	}

	public virtual void OnEntityHit(Entity entity, Vector2Int contactPosition) {
	}

	public virtual void OnEntityHitOnce(Entity entity) {
	}

	public virtual void OnObjectHit(WorldObject obj, Vector2Int contactPosition) {
	}

	public virtual void OnObjectHitOnce(WorldObject obj) {
	}

	public virtual void OnObjectHit(ActiveObject obj, Vector2Int contactPosition) {
	}

	public virtual void OnObjectHitOnce(ActiveObject obj) {
	}

	public virtual bool CanCast(Vector2Int castPosition, Vector2Int ownerPosition) {
		return castbox.Overlap(castPosition - ownerPosition);
	}

}
