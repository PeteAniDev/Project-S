using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BaseAttributes : MonoBehaviour {

	public int hp = 100;
	public int maxHp = 100;
	public int shield = 0;
	public float speed = 1;
	public float damageBonusP = 0;
	public float damageBonusM = 0;
	public float resistanceP = 0;
	public float resistanceM = 0;

	public void TakeDamage(BaseAttributes attacker, int damage, bool magic) {
		int trueDamage = (int)Mathf.Max((magic ? attacker.MagicDamageMultiplier() : attacker.PhysicDamageMultiplier()) * damage * (magic ? MagicResistanceMultiplier() : PhysicResistanceMultiplier()), 0);
		TakeTrueDamage(trueDamage);
	}

	public void TakeTrueDamage(int trueDamage) {
		shield -= trueDamage;
		hp += Mathf.Min(shield, 0);
		shield = Mathf.Max(shield, 0);
		hp = Mathf.Max(hp, 0);
	}

	public bool ShouldDie() {
		return hp <= 0;
	}

	public float PhysicDamageMultiplier() {
		return damageBonusP + 1;
	}

	public float MagicDamageMultiplier() {
		return damageBonusM + 1;
	}

	public float PhysicResistanceMultiplier() {
		return 1 - resistanceP;
	}

	public float MagicResistanceMultiplier() {
		return 1 - resistanceM;
	}

}
