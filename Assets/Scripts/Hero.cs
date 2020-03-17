using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character {
	public static Hero instance;
	public static float maxHeroHP = 100f;

	new void Awake() {
		instance = this;
		base.Awake();
	}

	protected override void Initialize() {
		this.maxStartingHP = Hero.maxHeroHP;
		base.Initialize();
		this.SetHP(Store.playerHealth);
	}

	public override void TakeDamage(float damage, bool instant) {
		base.TakeDamage(damage);
		Store.playerHealth = this.GetHP();
	}

	public override void FinishAttack() {
		base.FinishAttack();
	}

	public override void Heal(float _amount) {
		float amount = DemonicInfluence.Damned(_amount);
		base.Heal(amount);
	}
}
