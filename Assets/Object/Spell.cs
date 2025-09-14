using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spell {

	public Hitbox hitbox;
	public Entity owner;

	public void Cast(Vector2Int position) {
		OnCast(position);
		if (hitbox != null) {
			foreach (Vector2Int extend in hitbox.extends) {
				foreach (Entity entity in Entity.entities) {
					bool hit = false;
					foreach (Vector2Int entityExtend in entity.hitbox.extends) {
						if (position + extend == entity.gridPosition + entityExtend) {
							OnEntityHit(entity, position + extend);
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
						}
					}
					if (hit) {
						OnObjectHitOnce(obj);
					}
				}
				foreach (ActiveObject obj in ActiveObject.objects) {
					bool hit = false;
					foreach (Vector2Int objExtend in obj.hitbox.extends) {
						if (position + extend == obj.gridPosition + objExtend) {
							OnObjectHit(obj, position + extend);
						}
					}
					if (hit) {
						OnObjectHitOnce(obj);
					}
				}
			}
		}
	}

	public virtual void OnCast(Vector2Int position) {
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

}
