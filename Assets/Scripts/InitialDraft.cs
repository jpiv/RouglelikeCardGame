using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialDraft : MonoBehaviour {
	public GameObject container;
	public float[] dropModifiers;
	public int draftCount = 5;
	public int draftCards = 3;
	public bool addToDeck = true;

	protected float scale = 28f;

	private List<CardType> cards = new List<CardType>();
	private int currentDraft = 0;
	private CardType? includeCard;

	void Start() {
		this.CreateDraft();
	}

	public void SetIncludeCard(CardType includeCard) {
		this.includeCard = includeCard;
	}

	public void CreateDraft() {
		foreach (Transform child in this.container.transform) {
			Destroy(child.gameObject);
		}
		cards.Clear();
		List<CardType> randomDraftCards = this.GetDraftCards();

		if (this.includeCard != null) {
			randomDraftCards[Random.Range(0, randomDraftCards.Count)] = (CardType) this.includeCard;
		}
		for (int i = 0; i < this.draftCards; i++) {
			CardType card = randomDraftCards[i];
			this.MakeCard(card);
			this.cards.Add(card);
		}
	}

	List<CardType> GetDraftCards() {
		List<CardType> randomDraftCards = new List<CardType>();
		if (this.addToDeck) {
			for (int i = 0; i < this.draftCards; i++) {
				randomDraftCards.Add(Deck.instance.RandomCard(this.dropModifiers));
			}
		} else {
			randomDraftCards = Deck.instance.RandomCardsInDeck(this.draftCards);
		}
		return randomDraftCards;
	}

	void Draft(CardType card) {
		if (this.addToDeck) {
			Deck.instance.AddCardToFullDeck(card);
		} else {
			Deck.instance.RemoveCardFromFullDeck(card);	
		}
		CardWindow.instance.PopulateFullDeck();
		this.currentDraft++;
		if (this.currentDraft == this.draftCount) {
			this.EndDraft();
		} else {
			this.CreateDraft();
		}
	}

	public virtual void EndDraft() {
		WindowManager.ClearOverlays();
		SceneHelper.Open(SceneHelper.MAP);
		// WindowManager.ShowScreen(WindowManager.battleUI);
	}

    GameObject MakeCard(CardType cardType) {
    	GameObject layout = Creator.CreateLayoutElement(this.container.transform);
		GameObject cardObj = Creator.CreateCard(Vector3.zero, Vector3.zero, cardType, layout.transform, 1);
		Card card = cardObj.GetComponent<Card>();
		card.SetUICard(this.scale);

		card.SetClickAction(() => {
			this.Draft(card.cardType);
		});
		card.SetCustomHoverAction(() => {
		}, () => {});
		return cardObj;
    }
}
