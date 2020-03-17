using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour {
	public static Deck instance;
    public List<CardType> fullDeck = new List<CardType>();
    public List<CardType> deck = new List<CardType>();
    public List<CardType> discard = new List<CardType>();
    public CardType[] allCards;
    public CardType[] upgradedCards;

    private int _nextId = 0;

	void Awake() {
    	instance = this;
    	TextAsset deckData = Resources.Load<TextAsset>("cards");
    	DeckType cardData = JsonUtility.FromJson<DeckType>(deckData.text);
        this.allCards = cardData.cards;
        this.upgradedCards = cardData.upgradedCards;
    	this.FillDeck();
	}

    void FillDeck() {
        string[] initialHand = new string[5] { "Attack", "Attack", "Attack", "Brace", "Mend"};
        foreach (string name in initialHand) {
            CardType card = (CardType)this.GetCardByName(name);
            this.AddCardToFullDeck(card);
	    }
    }

    private int GetId() {
        int nextId = this._nextId;
        this._nextId++;
        return nextId;
    }

    public void NewDeck() {
        this.fullDeck.Clear();
        this.deck.Clear();
        this.discard.Clear();
        this.FillDeck();
    }

    private CardType ConstructCard(CardType card) {
        CardType newCard = card;
        EffectType[] effects = newCard.effects;
        newCard.effects = new EffectType[effects.Length];
        effects.CopyTo(newCard.effects, 0);
        return newCard;
    }

    private CardType? GetCardByName(string name) {
        foreach (CardType card in this.allCards) {
            if (card.name == name) return card;
        }
        return null;
    }

    private CardType? GetCardUpgrade(CardType baseCard) {
        foreach (CardType card in this.upgradedCards) {
            if (card.name == baseCard.upgrade) return card;
        }
        return null;
    }

    public bool HasUpgrade(CardType card) {
        return card.upgrade != null;
    }

    public void UpgradeCard(CardType card) {
        if (this.HasUpgrade(card)) {
            CardType UpgradedCard = (CardType)this.GetCardUpgrade(card);
            this.RemoveCardFromFullDeck(card);
            this.AddCardToFullDeck(UpgradedCard);
        }
    }

    public void AddCardToDeck(CardType card) {
        CardType deckCard = this.ConstructCard(card);
        this.deck.Add(deckCard);
    }

    public void AddCardToFullDeck(CardType card) {
        card.id = this.GetId();
        CardType deckCard = this.ConstructCard(card);
        CardType fullDeckCard = this.ConstructCard(card);
        this.deck.Add(deckCard);
        this.fullDeck.Add(fullDeckCard);
    }

    public void RemoveCardFromDeck(CardType card) {
        this.deck.RemoveAll(c => c.id == card.id);
        this.discard.RemoveAll(c => c.id == card.id);
    }

    public void RemoveCardFromFullDeck(CardType card) {
        this.deck.Remove(card);
        this.fullDeck.Remove(card);
    }

    public CardType RandomCard(float[] dropModifiers = null) {
        List<CardType>[] cardRarity = new List<CardType>[5];

        foreach (CardType card in this.allCards) {
        if (cardRarity[card.rarity - 1] == null) {
               cardRarity[card.rarity - 1] = new List<CardType>();
            }
            cardRarity[card.rarity - 1].Add(card);
        }

        int rarity = LootTable.GetRarity(dropModifiers);
        return cardRarity[rarity][Random.Range(0, cardRarity[rarity].Count)];
    }

    public List<CardType> RandomCardsInDeck(int numCards) {
        List<CardType> fullDeckCopy = new List<CardType>(this.fullDeck);
        List<CardType> randomCards = new List<CardType>();
        for (int i = 0; i < numCards; i++) {
            CardType card = fullDeckCopy[Random.Range(0, fullDeckCopy.Count)];
            fullDeckCopy.Remove(card);
            randomCards.Add(card);
        }
        return randomCards;
    }

    public void Reset() {
        this.discard.Clear();
        this.deck = new List<CardType>();
        for (int i = 0; i < this.fullDeck.Count; i++) {
            this.AddCardToDeck(this.fullDeck[i]);
        }
        this.Shuffle();
    }

    private void Shuffle() {
    	// Fisher-Yates Shuffle	
        int deckSize = this.deck.Count;
    	List<CardType> deckList = new List<CardType>(this.deck);
        this.deck = new List<CardType>();

    	for (int i = 0; i < deckSize; i++) {
    		int position = Random.Range(0, deckList.Count);
    		this.deck.Add(deckList[position]);
            deckList.RemoveAt(position);
    	}
    }

    private void RecycleDiscard() {
    	int discardSize = this.discard.Count;
    	for (int i = 0; i < discardSize; i++) {
    		CardType card = this.discard[0];
            this.discard.RemoveAt(0);
    		this.deck.Add(card);
    	}
    	this.Shuffle();
    }

    public void Discard(CardType card) {
    	discard.Add(card);
    }

    public CardType? Pop() {
        if (this.deck.Count == 0) return null;
        CardType card = this.deck[this.deck.Count - 1]; 
        this.deck.RemoveAt(this.deck.Count - 1);
        return (CardType?)card;
    }

    public CardType DrawCard() {
        CardType? card = this.Pop();
        if (card == null) {
			this.RecycleDiscard();
            return (CardType)this.Pop();
    	}
        return (CardType)card;
    }
}

[System.Serializable]
public struct DeckType {
   public CardType[] cards;
   public CardType[] upgradedCards;
}
