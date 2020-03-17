using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulCollector : MonoBehaviour {
	public Button RemoveButton;
	public Button UpgradeButton;

	private CardWindow cardWindow;
	private Card selectedCard;
	private int removeCost = 20;
	private int upgradeCost = 20;

    void Awake() {
    	cardWindow = GetComponent<CardWindow>(); 
    	RemoveButton.onClick.AddListener(this.RemoveCard);
		UpgradeButton.onClick.AddListener(this.UpgradeCard);
    }

    void Start() {
		this.OverwriteOnClick();
    }

    void OverwriteOnClick() {
    	foreach (Card card in this.cardWindow.cardObjs) {
    		card.SetClickAction(() => {
    			this.OnCardClick(card);
			});
	    }
    }

    void OnCardClick(Card card) {
    	if (this.selectedCard != null) {
	    	this.selectedCard.SetSelected(false);
	    }
	    if (this.selectedCard != null && this.selectedCard == card) {
	    	this.selectedCard = null;
	    	card.SetSelected(false);
	    	this.DisableButtons();
	    } else {
	    	this.selectedCard = card;
	        card.SetSelected(true);
	        this.EnableButtons();
	    }
    }

    void RemoveCard() {
    	if (this.selectedCard == null) return;
    	Store.AddSouls(-this.removeCost);
    	Deck.instance.RemoveCardFromFullDeck(this.selectedCard.cardType);
    	this.cardWindow.PopulateFullDeck();
    	this.OverwriteOnClick();
    	this.selectedCard = null;
    	this.DisableButtons();
    }

    void UpgradeCard() {
        if (this.selectedCard == null) return;
        Store.AddSouls(-this.removeCost);
        Deck.instance.UpgradeCard(this.selectedCard.cardType);
        this.cardWindow.PopulateFullDeck();
        this.OverwriteOnClick();
        this.selectedCard = null;
        this.DisableButtons();
    }

    void EnableButtons() {
    	if (Store.souls >= this.removeCost) {
	    	this.SetEnabledRemove(true);
    	}
        if (Store.souls >= this.upgradeCost && Deck.instance.HasUpgrade(this.selectedCard.cardType)) {
            this.SetEnabledUpgrade(true);
        }
    }

    void DisableButtons() {
    	this.SetEnabledUpgrade(false);	
    	this.SetEnabledRemove(false);	
    }

    void SetEnabledRemove(bool enabled) {
    	this.RemoveButton.interactable = enabled;
    }

    void SetEnabledUpgrade(bool enabled) {
    	this.UpgradeButton.interactable = enabled;
    }
}
