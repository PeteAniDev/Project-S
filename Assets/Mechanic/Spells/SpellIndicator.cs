using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SpellIndicator : MonoBehaviour {

	public static SpellIndicator instance;

	public SpellLoadout loadout = null;
	public int selection;
	public Entity owner;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		loadout.spells.Add(new Move());
		loadout.spells.Add(new Fireball());

		if (loadout != null) {
			loadout.owner = owner;
		}
	}

	private void Update() {
		if (loadout != null && loadout.Get(selection) != null) {
			Spell spell = loadout.Get(selection);
			spell.owner = owner;
			spell.RefreshCastBox();
			if (Input.GetMouseButtonDown(0) && spell.CanCast(GetMouseGridPosition(), owner.gridPosition)) {
				spell.Cast(GetMouseGridPosition());
			}
		}
	}

	private void OnDrawGizmos() {
		if (loadout == null) {
			return;
		}
		Spell spell = loadout.Get(selection);
		if (spell == null || spell.hitbox.extends == null) {
			return;
		}

		if (owner != null) {
			Gizmos.color = new Color(1, 1, 0, 0.4f);
			foreach (var extend in spell.castbox.extends) {
				Gizmos.DrawCube(GridLock.ToWorldPos(owner.gridPosition + extend), Vector3.one);
			}
		}

		Gizmos.color = new Color(0, 1, 1, 0.5f);
		foreach (var extend in spell.hitbox.extends) {
			Gizmos.DrawCube(GetMousePosition(extend), Vector3.one);
		}
	}

	private Vector2 GetMousePosition(Vector2Int offset) {
		return GridLock.ToWorldPos(GetMouseGridPosition() + offset);
	}

	private Vector2Int GetMouseGridPosition() {
		if (Camera.main == null) {
			return Vector2Int.zero;
		}
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return GridLock.ToGridPos(mouseWorldPos);
	}

}
