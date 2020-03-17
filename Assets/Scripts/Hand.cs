using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Rnd = UnityEngine.Random;
using UnityEngine.UI;

public class Hand : MonoBehaviour {
    public static Hand instance;
    public static bool isDragging;
	public GameObject cardPre;
	public int handSize = 5;
	public float cardSpacing = 0.5f;
	public float ySpacing = 1f;
	public float maxTilt = 45f;
    public bool selectionMode = false;
    public int selectionAmount = 0;
    public List<Card> cards = new List<Card>();
    public List<CardType> cardTypes = new List<CardType>();

    private float moveSpeed = 90.1f;
    private float scaleSpeed = 10.1f;
    private float rotateSpeed = 0.3f;
    private List<Card> selectedCards = new List<Card>();
    private GameObject hiddenCard;
    private bool isRendering = false;
    private bool forceReset = false;

    void Awake() {
    	instance = this;
    }

    void Update() {
        this.PositionCards();
    }

    public void DropCard(bool isTargeting = false) {
        foreach (Card card in this.cards) {
            if (card.isDragging) {
                this.ShowCard();
                card.canInteract = false;
                card.isDragging = false;
                if (isTargeting) {
                    Timer.TimeoutAction(() => {
                        card.canInteract = true;
                        Hand.isDragging = false;
                    }, 0.3f);
                } else {
                    card.SetInteractiveOnMouseUp();
                }
            }
        }
    }

    private void PositionCards() {
        if (this.isRendering) return;
        for (int i = 0; i < this.cards.Count; i++) {
            Card card = this.cards[i];
            Vector3 currentPos = card.transform.localPosition;
            Quaternion currentRotation = card.transform.localRotation;
            Quaternion targetRotation = card.defaultRot;
            if ((!card.isHovered && !card.isDragging) || this.forceReset) {
                card.ResetZIndex();
                card.transform.localPosition = Vector3.MoveTowards(
                    currentPos,
                    card.defaultPos,
                    Time.deltaTime * moveSpeed
                );
                card.transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, rotateSpeed);
                card.transform.localScale = Vector3.MoveTowards(
                    card.transform.localScale,
                    card.defaultScale,
                    Time.deltaTime * scaleSpeed
                );
            }
            if (currentPos == card.defaultPos && !card.canInteract) {
                card.ResetZIndex();
            }
        }
    }

    public int GetHandSize() {
        return this.cards.Count;
    }

    public List<Card> RandomCards(int amount) {
        List<Card> handCopy = new List<Card>(this.cards);
        List<Card> cardsPicked = new List<Card>();
        for (int i = 0; i < amount; i++) {
            Card rndCard = handCopy[Rnd.Range(0, handCopy.Count)];
            handCopy.Remove(rndCard);
            cardsPicked.Add(rndCard);
        }
        return cardsPicked;
    }

    private void ShowCard() {
        if (this.hiddenCard != null) {
            this.hiddenCard.SetActive(true);
        } 
    }

    private void HideCard(GameObject cardObject) {
        cardObject.SetActive(false);
        this.hiddenCard = cardObject;
    }

    private void RemoveCard(CardType card) {
        this.cardTypes.Remove(card);
        Deck.instance.Discard(card);
    }

    public void StartPlayCard(CardType playedCard, GameObject cardObject) {
        if (this.selectionMode) return;
        if (Crystals.instance.CanPlayCard(playedCard.mana)) {
            this.HideCard(cardObject);
            Battlefield.instance.StartPlayCard(playedCard);
        } else {
            Card card =  cardObject.GetComponent<Card>();
            Hand.isDragging = false;
            card.canInteract = true;
            card.isDragging = false;
        }
    }

    public void DisableCardPlay() {
        foreach (Card card in this.cards) {
            card.DisableInteraction();
            card.SetCustomHoverAction(() => {}, () => {});
        }
    }

    public void EnableCardPlay() {
        foreach (Card card in this.cards) {
            card.EnableInteraction();
        } 
    }

    public void CardSelection(int amount) {
        this.selectionMode = true;
        this.forceReset = true;
        this.selectionAmount = amount;
        AnimationQueue.Add(this.CreateCardSelection, 0f);
    }

    private void CreateCardSelection() {
        if (Battlefield.instance.gameOver) return;
        this.forceReset = false;
        GameObject selectionScreen = WindowManager.ShowScreen(WindowManager.discardChoose);
        Button confirmButton = selectionScreen.GetComponentInChildren<Button>();
        confirmButton.interactable = false;
        Hand.isDragging = false;
        foreach (Card card in this.cards) {
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
        Hand.isDragging = false;
        Crystals.instance.Consume(playedCard.mana);
        this.RemoveCard(playedCard);
        this.QueueHandRender();
    }

    public void DiscardCards(List<CardType> cards) {
        foreach (CardType card in cards) {
            this.RemoveCard(card);
        }
        this.QueueHandRender();
    }

    public void DiscardHand() {
        foreach (CardType card in new List<CardType>(this.cardTypes)) {
            this.RemoveCard(card);
        }
        this.QueueHandRender();
    }

    public void DrawHand() {
        for (int i = 0; i < handSize; i++) {
            this.DrawCard();
        }
    }

    public void DrawCard() {
        CardType newCard = Deck.instance.DrawCard();
        this.cardTypes.Add(newCard);
        this.QueueHandRender();
    }

    private void QueueHandRender() {
        List<CardType> cardsCopy = new List<CardType>(this.cardTypes);
        AnimationQueue.Add(() => {
            this.isRendering = true;
            this.RenderHand(cardsCopy);
        }, 0f);
        AnimationQueue.Add(() => { this.isRendering = false; }, 0f);
    }

    // Make this parameter mandatory when refactoring card physics
    public List<Card> RenderHand(List<CardType> nextCardTypes = null) {
        this.DropCard();
        this.cards.Clear();

		foreach (Transform child in this.transform) {
			Destroy(child.gameObject);
		}

        if (nextCardTypes.Count == 1) {
            GameObject card = Creator.CreateCard(Vector3.zero, Vector3.zero, (CardType)nextCardTypes[0], this.transform);
            this.cards.Add(card.GetComponent<Card>());
            return this.cards;
        } 
    	// -(x/(0.5 *len - 1) -1)^2 + 1
    	Func<float, int, float> yPosFn = (x, len) => -Mathf.Pow(x / (0.5f * (len - 1)) - 1, 2f) + 1;
    	// (x/0.5 - 1)^3
    	Func<float, int, float> rotationFn = (x, len) => ((2 * x) / (len - 1)) - 1;
    	float handWidth = GetComponent<BoxCollider2D>().size.x;
    	for (int i = 0; i < nextCardTypes.Count; i++) {
    		Vector3 pos = new Vector3(((float)i / (nextCardTypes.Count - 1)) * handWidth, yPosFn(i, nextCardTypes.Count) * ySpacing, -i);
    		Vector3 rotation = new Vector3(0, 0, rotationFn(i, nextCardTypes.Count) * -maxTilt);
            GameObject card = Creator.CreateCard(pos, rotation, (CardType)nextCardTypes[i], this.transform, i);
            this.cards.Add(card.GetComponent<Card>());
    	}
        return this.cards;
    }
}
