using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class Entity : QueueObject {

	public static List<Entity> entities = new List<Entity>();

	public int hp;
	public int maxHp;

	public override void OnEnable() {
		base.OnEnable();
		entities.Add(this);
	}

	public override void OnDisable() {
		entities.Remove(this);
		base.OnDisable();
	}

	public virtual void Damage(Entity entity, int damage) {
		entity.hp -= damage;
	}

	public virtual void Heal(Entity entity, int healing) {
		entity.hp += healing;
	}

	public abstract bool IsTeam(Entity owner);

	public void Push(Vector2Int direction) {
		gridPosition += direction;
	}

}
