using System.Collections.Generic;
using UnityEngine;

public class RockEl : Enemy {
	public GameObject tinyEl;

	private float spawnHP = 0f;
	private bool hasSplit = false;

	public override void TakeDamage(float damage, bool instant) {
		if (this.hasSplit) return;
		float baseHP = this.GetHP();
		base.TakeDamage(damage, instant);
		float splitPoint = 0.25f;
		float spawnHPMultiplier = 0.125f;
		if (this.GetHP() <= splitPoint * this.GetMaxHP()) {
			this.spawnHP = spawnHPMultiplier * baseHP;
			this.hasSplit = true;
			this.SetHP(0);
			this.OnDeath();
		}
	}	

	public override void OnDeath() {
		List<Enemy> spawnedEls = Battlefield.instance.AddEnemies(
			new List<GameObject>(new GameObject[2] { tinyEl, tinyEl })
		);
		foreach (Enemy spawnedEl in spawnedEls) {
			spawnedEl.ForceUpdateHP(this.spawnHP);
		}
	}
}