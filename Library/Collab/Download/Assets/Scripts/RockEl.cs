using System.Collections.Generic;
using UnityEngine;

public class RockEl : Enemy {
	private float spawnHP = 0f;
	private bool hasSplit = false;

	public override void TakeDamage(float damage) {
		float baseHP = this.GetHP();
		base.TakeDamage(damage);
		float splitPoint = 0.25f;
		float spawnHPMultiplier = 0.125f;
		if (this.GetHP() <= splitPoint * this.GetMaxHP() && !this.hasSplit) {
			this.spawnHP = spawnHPMultiplier * baseHP;
			this.SetHP(0f);
			this.hasSplit = true;
		}
	}	

	public override void OnDeath() {
		List<Enemy> spawnedEls = Battlefield.instance.AddEnemies(
			new List<GameObject>(new GameObject[2] { Enemies.allEnemies[2], Enemies.allEnemies[2] })
		);
		foreach (Enemy spawnedEl in spawnedEls) {
			Timer.TimeoutAction(() => spawnedEl.SetHP(this.spawnHP), 0f);
		}
	}
}