using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Phases {
	public static string PLAYER_START = "start";
	public static string PRE_ATTACK = "pre_attack";
    public static string POST_ATTACK = "post_attack";
	public static string ENEMY_START = "enemy_start";
	public static string ENEMY_PRE_ATTACK = "enemy_pre_attack";
    public static string ENEMY_POST_ATTACK = "enemy_post_attack";
}

public class Battlefield : MonoBehaviour {
	public static Battlefield instance;
	public bool waitingForSelection = false;
	public GameObject gameEndAnimation;
	public float nextDamage;
	public Character nextTarget;
    public List<CardType> nextTargetCards;
	public int turn = 0;
	public int cardsPlayed = 0;
	public bool playerTurn = true;
	public List<Effect> activeEffects = new List<Effect>();
    public CardType playedCard;

	private EnemyGroup enemyGroup;
	private bool gameOver = false;

	void Awake() {
		instance = this;  
	}

    void Start() {
		this.turn = 0;
		this.enemyGroup = new EnemyGroup();
    	Timer.TimeoutAction(Battlefield.instance.StartBattle, 0.1f);
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Hand.instance.RenderHand();
        	if (this.waitingForSelection) {
        		this.CancelSelection();
        	}
        }
    }

    public List<Enemy> AddEnemies(List<GameObject> newEnemies) {
    	return this.enemyGroup.AddEnemies(newEnemies);
    }

    public void StartBattle() {
    	Deck.instance.Reset();
    	this.StartPlayerTurn();
    }

    // PLAYER_START phase
    public void StartPlayerTurn() {
    	this.turn++;
        this.ExecuteEffects(Phases.PLAYER_START);
    	Crystals.instance.Refresh();
    	Hand.instance.DrawHand();
        AnimationQueue.Add(() => this.playerTurn = true, 0);
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
            this.ApplyCardEffects();
            if (playedCard.targetType != "card") {
    	    	this.ExecuteEffects(Phases.PRE_ATTACK);
            }
		} else if (playedCard.type == "attack") {
            if (playedCard.targetType != "aoe") {
    	    	this.TargetSelection();
            } else {
                this.AoEAttack();
            }
		}
    }

    public void AoEAttack() {
        Hero.instance.Attack();
        Hand.instance.FinishPlayCard(playedCard);
        this.nextDamage = this.playedCard.damage;
        foreach (Enemy target in this.enemyGroup.Iterate()) {
            this.nextTarget = target;
            this.ExecuteEffects(Phases.PRE_ATTACK);
        }
        foreach (Enemy target in this.enemyGroup.Iterate()) {
            this.nextTarget = target;
            this.AttackTarget(target, this.nextDamage, true, true);
        }
        // Buffer between all attack animations and card effects
            // Since attacks are all animated instantly in queue for AoE
        AnimationQueue.Add(() => {}, 1f);
        foreach (Enemy target in this.enemyGroup.Iterate()) {
            this.nextTarget = target;
            this.ApplyCardEffects(target);
            this.ExecuteEffects(Phases.POST_ATTACK);
        }
        this.RemoveDefeated();
    }

    public void CardTargetSelection(int amount) {
        this.waitingForSelection = true;
        Hand.instance.CardSelection(amount);
    }

    public void SelectCards(List<CardType> cards) {
        this.nextTargetCards = cards;
        this.ExecuteEffects(Phases.PRE_ATTACK);
        this.waitingForSelection = false;
    }

    public void TargetSelection() {
    	this.waitingForSelection = true;
    	TargetLine.instance.Show();
    }

    public void SelectTarget(Character target) {
    	if (this.waitingForSelection == true) {
    		this.waitingForSelection = false;
            this.nextDamage = this.playedCard.damage;
			this.nextTarget = target;
            TargetLine.instance.Hide();
            Hand.instance.FinishPlayCard(playedCard);
	    	this.ExecuteEffects(Phases.PRE_ATTACK);
            Hero.instance.Attack();
            this.AttackTarget(target, this.nextDamage);
            this.ApplyCardEffects(target);
            this.ExecuteEffects(Phases.POST_ATTACK);
            this.RemoveDefeated();
    	}
    }

    public void EndPlayerTurn() {
    	if (!this.playerTurn || this.gameOver || this.waitingForSelection) return;
    	this.playerTurn = false;
    	Hand.instance.DiscardHand();	
    	this.StartEnemyTurn();
    }

    // ENEMY_START phase
    public void StartEnemyTurn() {
    	this.RemoveEffects();
        
        this.ExecuteEffects(Phases.ENEMY_START);

        foreach (float damage in this.enemyGroup.Attack()) {
            this.nextDamage = damage;
            this.ExecuteEffects(Phases.ENEMY_PRE_ATTACK);
            this.AttackTarget(Hero.instance, this.nextDamage);
            this.ExecuteEffects(Phases.ENEMY_POST_ATTACK);
            this.RemoveDefeated();
        }
        this.EndEnemyTurn();
    }

    public void EndEnemyTurn() {
    	this.StartPlayerTurn();
    }

    public void HealTarget(Character target, float amount) {
    	target.Heal(amount);
    }

    public void AttackTarget(Character target, float damage, bool direct = true, bool instant = false) {
        float trueDamage = damage;
        if (direct && target != Hero.instance) {
           trueDamage = (float)damage + Store.GetPowerStat();
        }
    	target.TakeDamage(trueDamage, instant);
    }

    private void WinBattle() {
    	if (this.gameOver) return;
    	this.gameOver = true;
        AnimationQueue.Add(() =>
    		GameEndText.CreateText(this.gameEndAnimation, true, this.transform), 4f);
        AnimationQueue.Add(() => {
                Hand.instance.DiscardHand();
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

    public void ApplyEffect(EffectType effectType, Character target = null) {
        Type type = Effects.GetEffect(effectType.name);
        Effect effect = (Effect)Activator.CreateInstance(type, effectType);
        Action RemoveEffect = () => {
            this.activeEffects.RemoveAll(e => e.id == effect.id);
            print("Removing effect: " + effect.name);
        };
        print("Adding " + effect.name);
        this.activeEffects.Add(effect);
        Character effectTarget = effect.effect.targetType == "enemy" ? target : Hero.instance;
        effect.Apply(effectTarget, RemoveEffect);
    }

    private void ApplyCardEffects(Character target = null) {
    	if (this.playedCard.effects == null) return;
		foreach (EffectType effectType in this.playedCard.effects) {
            this.ApplyEffect(effectType, target);
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
