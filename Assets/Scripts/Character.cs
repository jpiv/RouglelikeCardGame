using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public GameObject healthBarGameObject;
	public GameObject attackAnimation;
	public GameObject damageAnimation;
	public List<Character> nextTargets;
	public float maxStartingHP = 20f;
	public bool dead = false;
	public int position = -1;

	private Animator animator;
	protected HealthBar health;
	protected List<Action> OnAttackTriggers = new List<Action>();
	protected List<Action> OnAttackFinishs = new List<Action>();

	public void Awake() {
		this.health = this.healthBarGameObject.GetComponent<HealthBar>();
		this.animator = GetComponent<Animator>();
	}

	public void Start() {
		this.Initialize();
	}

	protected virtual void Initialize() {
		this.health.SetMaxHP(this.maxStartingHP);
	}

	public void SetNextTargets(List<Character> targets) {
		this.nextTargets = targets;
	}

	public void AddBuff(Effect effect) {
		this.health.AddBuff(effect);
	}

	public void RemoveBuff(int id){
		this.health.RemoveBuff(id);
	}

	public void SetHP(float newHP) {
		this.health.SetHP(newHP);
	}

	public void ForceUpdateHP(float newHP) {
		this.health.ForceUpdateHP(newHP);
	}

	public void SetShield(float newShield) {
		this.health.SetShield(newShield);
	}

	public void AddShield(float shield) {
		this.SetShield(this.health.shield + shield);
	}

	public void RefreshHP() {
		this.health.RefreshHP();
	}

	public float GetHP() {
		return this.health.hp;
	}

	public float GetShield() {
		return this.health.shield;
	}

	public float GetMaxHP() {
		return this.health.maxHealth;
	}

	public void SetPosition(Vector3 pos, int posIndex) {
		this.transform.parent.localPosition = pos;
		this.position = posIndex;
	}

	private void CreateAttackAnimation() {
		GameObject attack = Instantiate(this.attackAnimation, this.transform);
		attack.transform.localPosition = new Vector3(0, 0, -2);
	}

	protected virtual void CreateDamageText(float damage) {
		Vector3 pos = new Vector3(6.56f, 1.26f, -2);
		Creator.CreateDamageText(damage, true, pos, this.transform);
	}

	protected virtual void CreateHealText(float amount) {
		Vector3 pos = new Vector3(6.56f, 1.26f, -2);
		Creator.CreateHealText(amount, true, pos, this.transform);
	}

	public virtual void TakeDamage(float damage, bool instant = false) {
		if (this.dead) return;
		float attackPause = instant ? 0f : 0.3f;
		float shieldDamage = this.health.shield - damage;
		this.SetShield(Mathf.Max(shieldDamage, 0f));
		float newHp = Mathf.Max(this.health.hp + Mathf.Min(shieldDamage, 0f), 0f);
		this.SetHP(newHp);
		AnimationQueue.Add(() => this.CreateAttackAnimation(), 0.0f);
		AnimationQueue.Add(() => this.CreateDamageText(damage), attackPause);
	}

	public virtual void Heal(float _amount) {
		float amount = (float)Mathf.Floor(_amount);
		float newHp = Mathf.Min(this.health.hp + amount, this.health.maxHealth);
		this.SetHP(newHp);
		AnimationQueue.Add(() => Creator.CreateHealAnimation(new Vector3(0.63f, -0.11f, -2), this.transform), 0);
		AnimationQueue.Add(() => this.CreateHealText(amount), 0f);
	}

    public virtual float Attack() {
    	this.OnAttackTriggers.Add(AnimationQueue.Add(
    		() => this.animator.SetBool("Attack", true)));
    	return 0f;
    }

    public virtual void TriggerAttack() {
    	this.OnAttackTriggers[0]();
    	this.OnAttackTriggers.RemoveAt(0);
    	this.OnAttackFinishs.Add(AnimationQueue.Add(() => {}));
    }

	public virtual void FinishAttack() {
    	this.OnAttackFinishs[0]();
    	this.OnAttackFinishs.RemoveAt(0);
    	this.animator.SetBool("Attack", false);
    }
}