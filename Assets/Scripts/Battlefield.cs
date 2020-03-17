using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public static class Phases {
	public static string PLAYER_START = "start";
    public static string CARD_PLAYED = "card_played";
	public static string PRE_ATTACK = "pre_attack";
    public static string POST_ATTACK = "post_attack";
    public static string POST_SELECT = "post_select";
    public static string POST_ABILITY = "post_ability";
	public static string ENEMY_START = "enemy_start";
	public static string ENEMY_PRE_ATTACK = "enemy_pre_attack";
    public static string ENEMY_POST_ATTACK = "enemy_post_attack";
}

public class Battlefield : MonoBehaviour {
	public static Battlefield instance;
	public bool waitingForSelection = false;
    public bool waitingForCardSelection = false;
	public GameObject gameEndAnimation;
	public int turn = 0;
	public int cardsPlayed = 0;
	public bool playerTurn = true;
    public bool gameOver = false;
    public float nextDamage;
    public List<CardType> nextTargetCards;
	public List<Effect> activeEffects = new List<Effect>();
    public CardType playedCard;

	private EnemyGroup enemyGroup;

	void Awake() {
		instance = this;  
	}

    void Start() {
		this.turn = 0;
		this.enemyGroup = new EnemyGroup();
        this.ApplyEnemyPassives();
    	Timer.TimeoutAction(Battlefield.instance.StartBattle, 0.1f);
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Hand.instance.DropCard(this.waitingForSelection);
        	if (this.waitingForSelection) {
        		this.CancelSelection();
        	}
        }
    }

    private void ApplyEnemyPassives() {
        foreach (Enemy enemy in this.enemyGroup.Iterate()) {
            this.ApplyEffects(enemy.GetPassiveEffects(), null, enemy);
        }
    }

    public List<Enemy> AddEnemies(List<GameObject> newEnemies) {
    	return this.enemyGroup.AddEnemies(newEnemies);
    }

    public void StartBattle() {
    	Deck.instance.Reset();
        if (DemonicInfluence.FearOfDemons()) {
            this.StartEnemyTurn();
        } else {
            this.StartPlayerTurn();
        }
    }

    // PLAYER_START phase
    public void StartPlayerTurn() {
    	this.turn++;
    	Crystals.instance.Refresh();
        AnimationQueue.Add(() => {
            this.playerTurn = true;
            Hand.instance.DrawHand();
        }, 0);
        this.ExecuteEffects(Phases.PLAYER_START);
    }

    public void CancelSelection() {
    	this.waitingForSelection = false;
    	TargetLine.instance.Hide();	
    }

    public void StartPlayCard(CardType playedCard) {
    	if (this.waitingForSelection) return;
    	this.playedCard = playedCard;
    	this.cardsPlayed++;
    	if (playedCard.type == "ability") {
            Hand.instance.FinishPlayCard(this.playedCard);
            this.ApplyEffects(this.playedCard.effects);
            if (playedCard.targetType != "card") {
    	    	this.ExecuteEffects(Phases.POST_ABILITY);
                this.ExecuteEffects(Phases.CARD_PLAYED);
            }
		} else if (playedCard.type == "attack") {
            if (playedCard.targetType != "aoe") {
    	    	this.TargetSelection();
            } else {
                Hand.instance.FinishPlayCard(this.playedCard);
                this.AOEAttack();
            }
		}
    }

    public void AOEAttack() {
        this.HeroAttack(this.enemyGroup.enemies.Select(e => (Character)e).ToList());
    }

    public void HeroAttack(List<Character> targets) {
        Hero.instance.SetNextTargets(targets);
        Hero.instance.Attack();

        foreach (Character target in targets) {
            this.nextDamage = this.playedCard.damage;
            this.ExecuteEffects(Phases.PRE_ATTACK);
            this.AttackTarget(target, this.nextDamage, true, true, false);
        }
        this.RemoveDefeated();
        // Buffer between all attack animations and card effects
            // Since attacks are all animated instantly in queue for AoE
        AnimationQueue.Add(() => {}, 1f);
        this.ApplyEffects(this.playedCard.effects, targets);
        this.ExecuteEffects(Phases.POST_ATTACK);
        this.ExecuteEffects(Phases.CARD_PLAYED);
    }

    public void CardTargetSelection(int amount) {
        this.waitingForCardSelection = true;
        Hand.instance.CardSelection(amount);
    }

    public void SelectCards(List<CardType> cards) {
        this.nextTargetCards = cards;
        this.ExecuteEffects(Phases.POST_SELECT);
        this.ExecuteEffects(Phases.CARD_PLAYED);
        this.waitingForCardSelection = false;
    }

    public void TargetSelection() {
    	this.waitingForSelection = true;
    	TargetLine.instance.Show();
    }

    public void SelectTarget(Character target) {
    	if (this.waitingForSelection == true) {
    		this.waitingForSelection = false;
            this.nextDamage = this.playedCard.damage;
            TargetLine.instance.Hide();
            Hand.instance.FinishPlayCard(playedCard);
            this.HeroAttack(new List<Character>() { target });
    	}
    }

    public void EndPlayerTurn() {
    	if (!this.playerTurn || this.gameOver || this.waitingForSelection || this.waitingForCardSelection) return;
    	this.playerTurn = false;
        Hand.instance.DisableCardPlay();
    	Hand.instance.DiscardHand();	
    	this.StartEnemyTurn();
    }

    // ENEMY_START phase
    public void StartEnemyTurn() {
    	this.RemoveEffects();
        
        this.ExecuteEffects(Phases.ENEMY_START);

        foreach (float damage in this.enemyGroup.Attack()) {
            EnemyGroup.attackingEnemy.SetNextTargets(new List<Character>() { Hero.instance });
            this.nextDamage = damage;
            this.ExecuteEffects(Phases.ENEMY_PRE_ATTACK);
            this.AttackTarget(Hero.instance, this.nextDamage);
            this.ApplyEffects(EnemyGroup.attackingEnemy.GetAttackEffects(), null, EnemyGroup.attackingEnemy);
            this.ExecuteEffects(Phases.ENEMY_POST_ATTACK);
        }
        this.EndEnemyTurn();
    }

    public void EndEnemyTurn() {
    	this.StartPlayerTurn();
    }

    public void HealTarget(Character target, float amount) {
    	target.Heal(amount);
    }

    public void AttackTarget(Character target, float damage, bool direct = true, bool instant = false, bool removeDefeated = true) {
        float trueDamage = damage;
        if (direct && target != Hero.instance) {
           trueDamage = (float)damage + Store.GetPowerStat();
        }
    	target.TakeDamage(trueDamage, instant);
        if (removeDefeated) {
            this.RemoveDefeated();
        }
    }

    private void WinBattle() {
    	if (this.gameOver) return;
    	this.gameOver = true;
        Hand.instance.DiscardHand();
        AnimationQueue.Add(() =>
    		GameEndText.CreateText(this.gameEndAnimation, true, this.transform), 2f);
        AnimationQueue.Add(() => {
                WindowManager.ShowScreen(WindowManager.lootUI);
        }, 0);
    }

    private void LoseBattle() {
    	if (this.gameOver) return;
    	this.gameOver = true;
        AnimationQueue.Add(() =>
    		GameEndText.CreateText(this.gameEndAnimation, false, this.transform), 4f);
		AnimationQueue.Add(App.ResetGame, 0);
    }

    private void RemoveDefeated() {
    	List<Enemy> defeated = this.enemyGroup.RemoveDefeated();
    	foreach (Enemy enemy in defeated) {
	    	this.RemoveEffects(enemy);
	    }
    	if (this.enemyGroup.CheckDefeat()) {
    		this.WinBattle();
		} else if (Hero.instance.GetHP() == 0) {
			this.LoseBattle();
		} 
    }

    public void RemoveEffects(Character target = null) {
    	List<Effect> effectsToRemove = new List<Effect>();
    	foreach (Effect effect in this.activeEffects) {
    		if (effect.target != Hero.instance && !this.enemyGroup.HasEnemy(effect.target)) {
    			effectsToRemove.Add(effect);
    		} else if (effect.ShouldRemove()) {
    			effectsToRemove.Add(effect);
    		}
    	}

    	foreach (Effect effect in effectsToRemove) {
    		effect.RemoveEffect();
    	}
    }

    public void ApplyEffect(EffectType effectType, List<Character> targets = null, Character originator = null) {
        Type type = Effects.GetEffect(effectType.name);
        List<Character> effectTargets = effectType.targetType == "enemy"
            ? targets
            : new List<Character>() { Hero.instance };
        foreach (Character target in effectTargets) { 
            Effect effect = (Effect)Activator.CreateInstance(type, effectType);
            Action RemoveEffect = () => {
                this.activeEffects.RemoveAll(e => e.id == effect.id);
            };
            this.activeEffects.Add(effect);
            Character effectTarget = target;
            effect.Apply(effectTarget, RemoveEffect, originator);
        }
    }

    private void ApplyEffects(EffectType[] effects, List<Character> targets = null, Character originator = null) {
    	if (effects == null) return;
		foreach (EffectType effectType in effects) {
            this.ApplyEffect(effectType, targets, originator);
		}
    }

    private void ExecuteEffects(string phase) {
    	List<Effect> effectsCopy = new List<Effect>(this.activeEffects);
    	foreach(Effect effect in effectsCopy) {
	    	if (effect.phases.Contains(phase)) {
                effect.Execute(phase);
	    	}
    	}
    }
}
