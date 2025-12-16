using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SpellLoadout : MonoBehaviour {

	public List<Spell> spells = new List<Spell>();
	public Entity owner;

	public void SetMaxSpellNumber(int number) {
		while (spells.Count > number) {
			spells.RemoveAt(spells.Count - 1);
		}
		while (spells.Count < number) {
			spells.Add(null);
		}
	}

	public void OverrideSpell(Spell spell, int index) {
		if (index < 0 || index >= spells.Count) {
			Debug.LogError("SpellLoadout: OverrideSpell: index out of range: " + index);
			return;
		}
		spells[index] = spell;
		spell.owner = owner;
	}

	public Spell Get(int index) {
		if (spells.Count <= index || index < 0) {
			return null;
		}
		return spells[index];
	}

}
