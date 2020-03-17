using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour {
	public GameObject cardPre;
	public int handSize = 5;
	public float cardSpacing = 0.5f;
	public float ySpacing = 1f;
	public float maxTilt = 45f;
    public bool selectionMode = false;
    public int selectionAmount = 0;
	public static Hand instance;

    private List<CardType> cardTypes = new List<CardType>();
    private List<Card> cards = new List<Card>();
    private List<Card> selectedCards = new List<Card>();

    void Awake() {
    	instance = this;
    }

    void Update() {
        // this.RenderHand();
    }

    private void HideCard(GameObject cardObject) {
        Destroy(cardObject);
    }

    private void RemoveCard(CardType card) {
        this.cardTypes.Remove(card);
        Deck.instance.Discard(card);
    }

    public void StartPlayCard(CardType playedCard, GameObject cardObject) {
        if (Crystals.instance.CanPlayCard(playedCard.mana)) {
            this.HideCard(cardObject);
            Battlefield.instance.StartPlayCard(playedCard);
        } else {
            this.RenderHand();
        }
    }

    public void CardSelection(int amount) {
        this.selectionMode = true;
        this.selectionAmount = amount;
        AnimationQueue.Add(this.CreateCardSelection, 0f);
    }

    private void CreateCardSelection() {
        GameObject selectionScreen = WindowManager.ShowScreen(WindowManager.discardChoose);
        Button confirmButton = selectionScreen.GetComponentInChildren<Button>();
        confirmButton.interactable = false;
        List<Card> cardsInHand = this.RenderHand();
        foreach (Card card in cardsInHand) {
            card.DisableInteraction();
            card.SetClickAction(() => {
                int selectedIndex = this.selectedCards.IndexOf(card);
                if (selectedIndex == -1) {
                    card.SetSelected();
                    this.selectedCards.Add(card);
                    if (this.selectedCards.Count == this.selectionAmount + 1) {
                        Card firstSelected = this.selectedCards[0];
                        this.selectedCards.RemoveAt(0);
                        firstSelected.SetSelected(false);
                    }
                    if (this.selectedCards.Count == this.selectionAmount) {
                        confirmButton.interactable = true;
                    } else {
                        confirmButton.interactable = false;
                    }
                } else {
                    confirmButton.interactable = false;
                    card.SetSelected(false);
                    this.selectedCards.RemoveAt(selectedIndex);
                }
            });
        }
    }

    public void OnConfirmSelection() {
        if (this.selectedCards.Count == this.selectionAmount) {
            this.selectionMode = false;
            List<CardType> selectedCardTypes = this.selectedCards.Select(card => card.cardType).ToList();
            WindowManager.HideScreen(WindowManager.discardChoose);
            Battlefield.instance.SelectCards(selectedCardTypes);
        }
    }

    public void FinishPlayCard(CardType playedCard) {
        Crystals.instance.Consume(playedCard.mana);
        this.RemoveCard(playedCard);
        this.RenderHand();
    }

    public void DiscardCards(List<CardType> cards) {
        foreach (CardType card in cards) {
            this.RemoveCard(card);
        }
        this.RenderHand();
    }

    public void DiscardHand() {
        foreach (CardType card in new List<CardType>(this.cardTypes)) {
            this.RemoveCard(card);
        }
        this.RenderHand();
    }

    public void DrawHand() {
        for (int i = 0; i < handSize; i++) {
            this.DrawCard();
        }
    }

    public void DrawCard() {
        CardType newCard = Deck.instance.DrawCard();
        this.cardTypes.Add(newCard);
        AnimationQueue.Add(() => this.RenderHand(), 0);
    }

    public List<Card> RenderHand() {
        Card.isDragging = false;
		foreach (Transform child in this.transform) {
			Destroy(child.gameObject);
		}

        this.cards = new List<Card>();
        if (this.cardTypes.Count == 1) {
            GameObject card = Creator.CreateCard(Vector3.zero, Vector3.zero, (CardType)this.cardTypes[0], this.transform);
            this.cards.Add(card.GetComponent<Card>());
            return this.cards;
        } 
    	// -(x/(0.5 *len - 1) -1)^2 + 1
    	Func<float, int, float> yPosFn = (x, len) => -Mathf.Pow(x / (0.5f * (len - 1)) - 1, 2f) + 1;
    	// (x/0.5 - 1)^3
    	Func<float, int, float> rotationFn = (x, len) => ((2 * x) / (len - 1)) - 1;
    	float handWidth = GetComponent<BoxCollider2D>().size.x;
    	for (int i = 0; i < this.cardTypes.Count; i++) {
    		Vector3 pos = new Vector3(((float)i / (this.cardTypes.Count - 1)) * handWidth, yPosFn(i, this.cardTypes.Count) * ySpacing, -i);
    		Vector3 rotation = new Vector3(0, 0, rotationFn(i, this.cardTypes.Count) * -maxTilt);
            GameObject card = Creator.CreateCard(pos, rotation, (CardType)this.cardTypes[i], this.transform, i);
            this.cards.Add(card.GetComponent<Card>());
    	}
        return this.cards;
    }
}
