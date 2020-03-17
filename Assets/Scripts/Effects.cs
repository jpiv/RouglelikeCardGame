using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public struct EffectType {
	public string name;
	public string image;
	public string targetType;
	public string[] phases;
	public bool cloned;
	public ModifierType modifiers;
}

[System.Serializable]
public struct ModifierType {
	public int turns;
	public int hits;
	public float damageMultiplier;
	public float hpBelow;
	public float amount;
	public bool trance;
	public bool deathBlow;
	public bool onShieldDamage;
	public bool random;
	public bool whenHit;
}

public class Effect {
	public static int nextId = 0;
	public int id;
	public string name;
	public List<string> phases;
	public List<Character> targets;
	public Character target;
	public Character originator;
	public EffectType effect;
	protected ModifierType modifiers;
	protected int turnApplied;
	protected int playIndex;
	protected Action _RemoveEffect;

	public Effect(EffectType effectType) {
		this.phases = effectType.phases.Select(phase => (string)typeof(Phases).GetField(phase).GetValue(null)).ToList();
		this.modifiers = effectType.modifiers;
		this.effect = effectType;
		this.id = Effect.nextId;
		Effect.nextId++;
	}

	public void Execute(string phase) {
		if (this.modifiers.whenHit && this.target.nextTargets.Contains(this.originator)) {
			this._Execute(phase);
		} else if (!this.modifiers.whenHit) {
			this._Execute(phase);
		}
	}

	public virtual void _Execute(string phase) {
	}

	protected void TargetAction(Action<Character> action) {
		foreach (Character target in this.targets) {
			action(target);
		}
	}

	public void Apply(Character target, Action RemoveEffect, Character originator = null) {
		this.target = target;
		this.originator = originator ?? Hero.instance;
		this.turnApplied = Battlefield.instance.turn;
		this.playIndex = Battlefield.instance.cardsPlayed;
		this._RemoveEffect = RemoveEffect;
		if (this.effect.image != null) {
			target.AddBuff(this);
		}

		// Replace amount with # of cards in hand
		if (this.modifiers.amount == -1) {
			this.modifiers.amount = Hand.instance.cardTypes.Count;
		// Replace amount with # of mana availible
		} else if (this.modifiers.amount == -2) {
			this.modifiers.amount = Crystals.instance.crystals;
		}

		if (this.modifiers.deathBlow && this.effect.targetType == "self" && !this.effect.cloned) {
			List<Character>	nextTargets = this.originator.nextTargets;
			List<Character> deadTargets = nextTargets.FindAll(t => t.dead);		
			foreach (Character deadTarget in deadTargets) {
				this.Clone();
			}
			this.RemoveEffect();
		} else if (!this.modifiers.deathBlow || this.effect.cloned) {
			this._Apply();
		} else {
			this.RemoveEffect();
		}
	}

	public virtual void _Apply() {

	}

	public EffectType Clone() {
		EffectType newEffectType = this.effect;
		newEffectType.cloned = true;
		Battlefield.instance.ApplyEffect(newEffectType, new List<Character>() { this.target });
		return newEffectType;
	}

	public virtual bool ShouldRemove() { return false; }

	public bool IsShieldDamage() {
		return Hero.instance.GetShield() > 0;
	}

	public void RemoveEffect() {
		if (this.modifiers.trance) return;
		if (this.effect.image != null) {
			this.target.RemoveBuff(this.id);
		}
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
		{ "dot", typeof(DoT) },
		{ "mana", typeof(Mana) },
		{ "mutateAttack", typeof(MutateAttack) },
		{ "reduceDamage", typeof(ReduceDamage) },
		{ "heal", typeof(Heal) },
		{ "shield", typeof(Shield) },
		{ "mark", typeof(Mark) },
		{ "discard", typeof(Discard) },
		{ "dissolve", typeof(Dissolve) },
		{ "draw", typeof(Draw) },
		{ "multiStrike", typeof(MultiStrike) }
	};

	public static Type GetEffect(string effect) {
		return Effects.effects[effect];
	}
}

public class Dissolve : Effect {
	public Dissolve(EffectType effectType) : base(effectType) {
		this.name = "dissolve";
	}

	public override void _Execute(string phase) {
		CardType card = Battlefield.instance.playedCard;
		List<EffectType> cardEffects = card.effects.ToList();
		EffectType et = cardEffects.Find(e => e.name == this.name);
		int effectIndex = cardEffects.IndexOf(et);
		card.effects[effectIndex].modifiers.amount--;
		if (card.effects[effectIndex].modifiers.amount == 0) {
			Deck.instance.RemoveCardFromDeck(card);
		}
		this.RemoveEffect();
	}
}

public class Discard : Effect {
	public Discard(EffectType effectType) : base(effectType) {
		this.name = "discard";
	}

	public override void _Apply() {
		if (Hand.instance.GetHandSize() < this.modifiers.amount){
			this.RemoveEffect();
		} else if (!this.modifiers.random) {
			Battlefield.instance.CardTargetSelection((int)this.modifiers.amount);
		}
	}

	public override void _Execute(string phase) {
		List<CardType> cardsToDiscard;
		if (this.modifiers.random) {
			cardsToDiscard = Hand.instance.RandomCards((int)this.modifiers.amount).Select(card => card.cardType).ToList();
			Hand.instance.DiscardCards(cardsToDiscard);
			this.RemoveEffect();
		} else if (!this.modifiers.random && phase == Phases.POST_SELECT) {
			cardsToDiscard = Battlefield.instance.nextTargetCards;
			Hand.instance.DiscardCards(cardsToDiscard);
			this.RemoveEffect();
		}
	}
}

public class Mark : Effect {
	private int markCount = 3;
	private float markDamage = 5f;
	private bool applyAfterAttack = false;
	public Mark(EffectType effectType) : base(effectType) {
		this.name = "mark";
	}

	public override void _Apply() {
		if (!this.modifiers.trance && !this.modifiers.onShieldDamage) {
			this.CheckMarks();
			if (!this.effect.cloned) {
				for (int i = 1; i < this.modifiers.amount; i++) {
					this.Clone();
				}
			}
		}
	}

	public override void _Execute(string phase) {
		if (this.modifiers.trance && phase == Phases.POST_ATTACK) {
			this.AddMarks(this.originator.nextTargets);
		}
		if (this.modifiers.onShieldDamage) {
			if (this.IsShieldDamage() && phase == Phases.ENEMY_PRE_ATTACK) {
				this.applyAfterAttack = true;
			}
			if (this.applyAfterAttack && phase == Phases.ENEMY_POST_ATTACK) {
				this.AddMark(EnemyGroup.attackingEnemy);
			} else if (!this.IsShieldDamage()) {
				this.RemoveEffect();	
			}
		}
	}

	private void AddMarks(List<Character> targets) {
		foreach (Character target in targets) {
			this.AddMark(target);
		}
	}

	private void AddMark(Character target) {
		EffectType mark = this.CreateMark();
		Battlefield.instance.ApplyEffect(mark, new List<Character>(){ target });	
	}

	private EffectType CreateMark() {
		EffectType mark = new EffectType();
		mark.name = this.name;
		mark.image = "mark";
		mark.targetType = "enemy";
		mark.modifiers = new ModifierType();
		mark.phases = new string[0];
		return mark;
	}

	private void Detonate(List<Mark> foundMarks) {
		Battlefield.instance.AttackTarget(this.target, this.markDamage, false);
		for (int i = 0; i < this.markCount; i++) {
			foundMarks[i].RemoveEffect();
		}
	}

	private void CheckMarks() {
		List<Mark> marks = this.GetMarks();
		if (marks.Count >= this.markCount) {
			this.Detonate(marks);
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

public class MultiStrike : Effect {
	private static int timesAttacked;
	public MultiStrike(EffectType effectType) : base(effectType) {
		this.name = "multiStrike";
	}

	private bool HasEffect() {
		List<Effect> effects = Battlefield.instance.activeEffects;
		return effects.FindAll(effect => effect is MultiStrike).Count > 1;
	}

	public override void _Execute(string phase) {
		this.RemoveEffect();
		if (timesAttacked == this.modifiers.amount - 1) {
			timesAttacked = 0;
		} else {
			timesAttacked++;
			if (Battlefield.instance.playedCard.targetType == "aoe") {
				Battlefield.instance.AOEAttack();
			} else {
				Battlefield.instance.HeroAttack(this.originator.nextTargets);
			}
		}
	}
}

public class Shield : Effect {
	public Shield(EffectType effectType) : base(effectType) {
		this.name = "shield";
	}

	public override void _Execute(string phase) {
		Hero.instance.AddShield(this.modifiers.amount);
		this.RemoveEffect();
	}
}

public class Heal : Effect {
	public Heal(EffectType effectType) : base(effectType) {
		this.name = "heal";
	}

	public override void _Execute(string phase) {
		Battlefield.instance.HealTarget(this.target, this.modifiers.amount);
		this.RemoveEffect();
	}
}

public class Draw : Effect {
	public Draw(EffectType effectType) : base(effectType) {
		this.name = "draw";
	}

	public override void _Execute(string phase) {
		for (int i = 0; i < this.modifiers.amount; i++) {
			Hand.instance.DrawCard();
		}
		this.RemoveEffect();
	}
}

public class MutateAttack : Effect {
	public MutateAttack(EffectType effectType) : base(effectType) {
		this.name = "mutateAttack";
	}

	public override bool ShouldRemove() {
		return (
			(this.modifiers.turns == 0 && !Battlefield.instance.playerTurn) || 
			((this.turnApplied + this.modifiers.turns) < Battlefield.instance.turn)
		);
	}

	public override void _Execute(string phase) {
		if (this.OnPlayedCard()) {
			return;
		} else if (!this.OnNextCard()) {
			RemoveEffect();
			return;
		}
		foreach (Character target in this.originator.nextTargets) {
			bool hpCondition = target.GetMaxHP() * this.modifiers.hpBelow >= target.GetHP();
			if (hpCondition && this.modifiers.damageMultiplier != 0) {
				Battlefield.instance.nextDamage *= this.modifiers.damageMultiplier;
			}
		}
		RemoveEffect();
	}
}


public class Mana : Effect {
	public Mana(EffectType effectType) : base(effectType) {
		this.name = "mana";
	}

	public override void _Execute(string phase) {
		Crystals.instance.AddMana((int)this.modifiers.amount);
		this.RemoveEffect();
	}
}

public class ReduceDamage : Effect {
	private int hitsActive = 0;
	public ReduceDamage(EffectType effectType) : base(effectType) {
		this.name = "reduceDamage";
	}

	public override void _Execute(string phase) {
		if (this.hitsActive < this.modifiers.hits) {
			Battlefield.instance.nextDamage *= this.modifiers.damageMultiplier;
			this.hitsActive++;
		}
		if (this.hitsActive >= this.modifiers.hits) {
			RemoveEffect();
		}
	}
}

public class DoT : Effect {
	private int turnsActive = 0;
	public DoT(EffectType effectType) : base(effectType) {
		this.name = "dot";
	}

	public override void _Execute(string phase) {
		Battlefield.instance.AttackTarget(this.target, this.modifiers.amount, false);
		this.turnsActive++;

		if (this.turnsActive == this.modifiers.turns) {
			RemoveEffect();
		}
	}
}
