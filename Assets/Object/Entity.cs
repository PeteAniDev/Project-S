using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class Entity : QueueObject {

	public static List<Entity> entities = new List<Entity>();

	public Hitbox hitbox;
	public int hp;
	public int maxHp;

	private void OnEnable() {
		entities.Add(this);
	}

	private void OnDisable() {
		entities.Remove(this);
	}

}
