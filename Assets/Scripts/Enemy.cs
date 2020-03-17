using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class Enemy : Character {
    private static float scale = 1f;
    public string enemyName = "enemy";

    private EnemyType type;
    private AttackType attack;

    public static void SetScale(float scale) {
        Enemy.scale = scale;
    }

    public void SetType(EnemyType type) {
        this.type = type;
    }

    protected override void Initialize() {
        if (this.type.health != 0) {
            this.maxStartingHP = this.type.health;
        }
        this.health.SetMaxHP(this.maxStartingHP * (1f + Enemy.scale));
    }

    void OnMouseUpAsButton() {
        if (this.GetHP() == 0) return;
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

    private int ScaleDamage(int damage) {
        return (int)((float)damage * (1 + Enemy.scale * 2));
    }

    public AttackType PickAttack() {
        AttackType attack = this.type.attacks[Rnd.Range(0, this.type.attacks.Length)];
        this.attack = attack;
        return attack;
    }

    public EffectType[] GetAttackEffects() {
        return this.attack.effects;
    }

    public EffectType[] GetPassiveEffects() {
        return this.type.passives;
    }

    private int GetAttackDamage() {
        return Rnd.Range(this.ScaleDamage(this.attack.damageMin), this.ScaleDamage(this.attack.damageMax));
    }

    public override float Attack() {
        int damage = this.GetAttackDamage();
        base.Attack();
        return damage;
    }
}

[System.Serializable]
public struct EnemyType {
    public string name;
    public int health;
    public AttackType[] attacks;
    public EffectType[] passives;
}

[System.Serializable]
public struct AttackType {
    public int damageMin;
    public int damageMax;
    public EffectType[] effects;
}
