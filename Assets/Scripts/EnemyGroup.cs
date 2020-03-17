using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unity = UnityEngine.Object;

public class EnemyGroup {
	public static EnemyGroup instance;
	public static Enemy attackingEnemy;
	public List<Enemy> enemies;

	public EnemyGroup() {
		instance = this;
		this.SpawnEnemies();
	}

	public void RefreshHP() {
		foreach (Enemy enemy in this.enemies) {
			enemy.RefreshHP();
		}
	}

	private void SpawnEnemies() {
		if (LevelGraph.instance.IsFinalRoom()) {
			Enemy boss = Enemies.SpawnBoss(EnemyPositions.FOUR[3]);
			this.enemies = new List<Enemy>() { boss };
		} else {
			List<Enemy> enemies = Enemies.SpawnEnemies(EnemyPositions.TWO);
			this.enemies = enemies;
		}
		this.Reposition(this.enemies);
	}

	public List<Enemy> AddEnemies(List<GameObject> newEnemies) {
		List<Enemy> spawnedEnemies = new List<Enemy>();
		foreach (GameObject enemy in newEnemies) {
			Enemy spawnedEnemy = Enemies.CreateEnemy(Vector3.zero, enemy);
			spawnedEnemies.Add(spawnedEnemy);
			this.enemies.Add(spawnedEnemy);
		}
		return spawnedEnemies;
	}

	public void Reposition(List<Enemy> currentEnemies) {
		List<int> openPositions = new List<int>();
		for (int i = 0; i < EnemyPositions.FOUR.Length; i++) {
			Enemy enemy = currentEnemies.Find(e => e.position == i);
			if (enemy == null) {
				openPositions.Add(i);
			}
		}
		foreach (int pos in openPositions) {
			Enemy enemy = currentEnemies.Find(e => e.position == -1);
			if (enemy != null) {
				enemy.SetPosition(EnemyPositions.FOUR[pos], pos);
			}
		}
	}

	public void RemoveEnemy(Enemy enemy) {
		enemy.dead = true;
		this.enemies.Remove(enemy);
	}

	public bool HasEnemy(Character enemy) {
		return this.enemies.Contains(enemy);
	}

	public List<Enemy> RemoveDefeated() {
		List<Enemy> enemiesToRemove = new List<Enemy>();
		foreach (Enemy enemy in this.enemies) {
			if (enemy.GetHP() == 0) {
				enemiesToRemove.Add(enemy);
			}
		};
		foreach (Enemy enemy in enemiesToRemove) {
			this.RemoveEnemy(enemy);
		}

		float deathTimeout = 0.8f;
		List<Enemy> enemiesCopy = new List<Enemy>(this.enemies);
		AnimationQueue.Add(() => {}, deathTimeout);
		AnimationQueue.Add(() => {
			foreach (Enemy enemy in enemiesToRemove) {
				unity.Destroy(enemy.gameObject);
			}
			this.Reposition(enemiesCopy);
		}, 0);
		return enemiesToRemove;
	}

	public bool CheckDefeat() {
		return this.enemies.Count == 0;
	}

	public IEnumerable Iterate() {
		List<Enemy> enemiesCopy = new List<Enemy>(this.enemies);
		foreach (Enemy enemy in enemiesCopy) {
			yield return enemy;
		};
	}

	public IEnumerable Attack() {
		List<Enemy> enemiesCopy = new List<Enemy>(this.enemies);
		foreach (Enemy enemy in enemiesCopy) {
			EnemyGroup.attackingEnemy = enemy;
			enemy.PickAttack();
			float damage = enemy.Attack();
			yield return damage;
		};
	}
}
