using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unity = UnityEngine.Object;

public class EnemyGroup {
	private List<Enemy> enemies;

	public EnemyGroup() {
		List<Enemy> enemies = Enemies.SpawnEnemies(EnemyPositions.TWO);
		this.enemies = enemies;
	}

	public void RefreshHP() {
		foreach (Enemy enemy in this.enemies) {
			enemy.RefreshHP();
		}
	}

	public List<Enemy> AddEnemies(List<GameObject> newEnemies) {
		List<Enemy> spawnedEnemies = new List<Enemy>();
		foreach (GameObject enemy in newEnemies) {
			Enemy spawnedEnemy = Enemies.CreateEnemy(Vector3.zero, enemy);
			spawnedEnemies.Add(spawnedEnemy);
			this.enemies.Add(spawnedEnemy);
		}
		this.Reposition();
		return spawnedEnemies;
	}

	public void Reposition() {
		for (int i = 0; i < this.enemies.Count; i++) {
			this.enemies[i].SetPosition(EnemyPositions.counts[this.enemies.Count - 1][i]);
		}
	}

	public void RemoveEnemy(Enemy enemy) {
		this.enemies.Remove(enemy);		
		unity.Destroy(enemy.gameObject);
		enemy.OnDeath();
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
		return enemiesToRemove;
	}

	public bool CheckDefeat() {
		return this.enemies.Count == 0;
	}

	public IEnumerable StartAttack(Character.AnimationFinishDel OnAttackFinish) {
		// Bug when breaking down rock ele from bleed
		foreach (Enemy enemy in this.enemies) {
			enemy.StartAttack(damage => {
				OnAttackFinish(damage);
			});
			yield return enemy;
		};
	}

	public IRnumerable Attack() {
		foreach (Enemy enemy in this.enemies) {
			flaot damage = enemy.Attack();
			yield return damage;
		};
	}
}
