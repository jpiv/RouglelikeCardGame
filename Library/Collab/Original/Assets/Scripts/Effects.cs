using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectType {
	public string name;
	public string image;
	public ModifierType modifiers;
}

[System.Serializable]
public class ModifierType {
	public int turns;
	public int hits;
	public float damageMultiplier;
	public float hpBelow;
	public float amount;
}

public class Effect {
	public static int nextId = 0;
	public int id;
	public string name;
	public List<string> phases;
	public Character target;
	public EffectType effect;
	protected ModifierType modifiers;
	protected int turnApplied;
	protected int playIndex;
	protected Action _RemoveEffect;

	public Effect(EffectType effectType, params string[] phases) {
		this.phases = new List<string>(phases);
		this.modifiers = effectType.modifiers;
		this.effect = effectType;
		this.id = Effect.nextId;
		Effect.nextId++;
	}

	public virtual void Execute() {
		Debug.Log("Execting effect");
	}

	public virtual void Apply(Character target, Action RemoveEffect) {
		this.target = target;
		this.turnApplied = Battlefield.instance.turn;
		this.playIndex = Battlefield.instance.cardsPlayed;
		this._RemoveEffect = RemoveEffect;
		if (this.effect.image != null) {
			this.target.AddBuff(this);
		}
	}

	public virtual bool ShouldRemove() { return false; }

	public void RemoveEffect() {
		Debug.Log("removing effect " + this.name);
		this.target.RemoveBuff(this.id);
		this._RemoveEffect();
	}

	protected bool OnNextCard() {
		return (this.playIndex + 1) == Battlefield.instance.cardsPlayed;
	}

	protected bool OnPlayedCard() {
		return this.playIndex == Battlefield.instance.cardsPlayed;
	}
}


static class Effects {
	public static Dictionary<string, Type> effects = new Dictionary<string, Type>()
	{
		{ "bleed", typeof(Bleed) },
		{ "mutateAttack", typeof(MutateAttack) },
		{ "reduceDamage", typeof(ReduceDamage) },
		{ "heal", typeof(Heal) },
		{ "mark", typeof(Mark) }
	};

	public static Type GetEffect(string effect) {
		return Effects.effects[effect];
	}
}

public class Mark : Effect {
	private int markCount = 3;
	private float markDamage = 5f;
	private bool detonate = false;
	public Mark(EffectType effectType) : base(effectType, Phases.POST_ATTACK, Phases.ENEMY_ATTACK) {
		this.name = "mark";
	}

	public override void Apply(Character target, Action RemoveEffect) {
		base.Apply(target, RemoveEffect);
		this.CheckMarks();
	}

	public override void Execute() {
		if (this.detonate) {
			List<Mark> marks = this.GetMarks();
			this.Detonate(marks);
		}
	}

	private void Detonate(List<Mark> foundMarks) {
		Battlefield.instance.AttackTarget(this.target, this.markDamage);
		foreach (Mark mark in foundMarks) {
			mark.RemoveEffect();
		}
	}

	private void CheckMarks() {
		List<Mark> marks = this.GetMarks();
		if (marks.Count >= this.markCount) {
			this.detonate = true;
		}
	}

	private List<Mark> GetMarks() {
		List<Effect> effects = Battlefield.instance.activeEffects;
		List<Mark> foundMarks = new List<Mark>();
		foreach (Effect effect in effects) {
			if (effect is Mark && effect.target == this.target) {
				foundMarks.Add((Mark)effect);
			}
		}
		return foundMarks;
	}
}

public class Heal : Effect {
	public Heal(EffectType effectType) : base(effectType, Phases.PRE_ATTACK) {
		this.name = "heal";
	}

	public override void Execute() {
		Battlefield.instance.HealTarget(this.target, this.modifiers.amount);
		this.RemoveEffect();
	}
}

public class MutateAttack : Effect {
	public MutateAttack(EffectType effectType) : base(effectType, Phases.PRE_ATTACK) {
		this.name = "mutateAttack";
	}

	public override bool ShouldRemove() {
		return (
			(this.modifiers.turns == 0 && !Battlefield.instance.playerTurn) || 
			((this.turnApplied + this.modifiers.turns) < Battlefield.instance.turn)
		);
	}

	public override void Execute() {
		if (this.OnPlayedCard()) {
			return;
		} else if (!this.OnNextCard()) {
			RemoveEffect();
			return;
		}
		bool hpCondition = Battlefield.instance.nextTarget.GetMaxHP() * this.modifiers.hpBelow >= Battlefield.instance.nextTarget.GetHP();
		if (hpCondition && this.modifiers.damageMultiplier != 0) {
			Battlefield.instance.nextDamage *= this.modifiers.damageMultiplier;
		}
		RemoveEffect();
	}
}

public class ReduceDamage : Effect {
	private int hitsActive = 0;
	public ReduceDamage(EffectType effectType) : base(effectType, Phases.ENEMY_ATTACK) {
		this.name = "reduceDamage";
	}

	public override void Execute() {
		if (this.hitsActive < this.modifiers.hits) {
			Battlefield.instance.nextDamage *= this.modifiers.damageMultiplier;
			this.hitsActive++;
		}
		if (this.hitsActive >= this.modifiers.hits) {
			RemoveEffect();
		}
	}
}

public class Bleed : Effect {
	private int turnsActive = 0;
	public Bleed(EffectType effectType) : base(effectType, Phases.ENEMY_START) {
		this.name = "bleed";
	}

	public override void Execute() {
		Battlefield.instance.AttackTarget(this.target, 2f);

		this.turnsActive++;
		if (this.turnsActive == this.modifiers.turns) {
			RemoveEffect();
		}
	}
}
