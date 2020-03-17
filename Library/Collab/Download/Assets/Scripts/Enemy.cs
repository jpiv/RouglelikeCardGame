using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class Enemy : Character {
    public int damageMin = 7;
    public int damageMax = 10;

    void OnMouseUpAsButton() {
    	Battlefield.instance.SelectTarget(this);
    }

    public virtual void OnDeath() {}

    protected override void CreateDamageText(float damage) {
		Vector3 pos = new Vector3(-5.2f, 1.43f, -2);
		Creator.CreateDamageText(damage, false, pos, this.transform);
    }

    protected override void CreateHealText(float amount) {
        Vector3 pos = new Vector3(6.56f, 1.26f, -2);
        Creator.CreateHealText(amount, true, pos, this.transform);
    }

    public override void FinishAttack() {
        int damage = Rnd.Range(this.damageMin, this.damageMax);
    	this.OnAttackFinish(damage);
    	this.OnAttackFinish = null;
    	base.FinishAttack();
    }

    public override float Attack() {
        int damage = Rnd.Range(this.damageMin, this.damageMax);
        base.Attack();
        return damage;
    }
}
