using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWindow : MonoBehaviour {
	public static CardWindow instance;
    public bool useUILayer = false;
	public GameObject content;
	public GameObject scrollView;
    public List<Card> cardObjs;

	private List<CardType> cards;
	private GameObject thumb;

	void Awake() {
        if (instance == null) {
    		instance = this;
        }
	}

    void Start() {
        this.PopulateFullDeck();
    }

	public void Toggle() {
		if (this.scrollView.activeSelf) {
			this.scrollView.SetActive(false);
		} else {
			this.scrollView.SetActive(true);
            this.PopulateCards();
		}
	}

    public void PopulateFullDeck() {
        this.cards = new List<CardType>(Deck.instance.fullDeck);
        this.PopulateCards();
    }

	public void PopulateDeck() {
		this.cards = new List<CardType>(Deck.instance.deck);
        this.PopulateCards();
	}

	public void PopulateDiscard() {
		this.cards = new List<CardType>(Deck.instance.discard);
        this.PopulateCards();
	}

    public void PopulateCards() {
    	this.ClearCards();
    	int deckSize = this.cards.Count;
    	int colCount = 4;
    	Rect rect = this.scrollView.GetComponent<RectTransform>().rect;
    	RectTransform contentRect = this.content.GetComponent<RectTransform>();
    	float scale = 9f;
    	float xSpacing = rect.width / colCount;
    	float contentHeight= 0f;

    	for (int i = 0; i < deckSize; i++) {
    		CardType card = this.cards[i];
    		GameObject cardObj = this.MakeCard(card);
	    	Vector3 cardRect = cardObj.GetComponent<Card>().cardSprite.GetComponent<SpriteRenderer>().sprite.bounds.size * scale;
            cardObj.transform.localScale = Vector3.one * scale;
	    	float xMargin = (rect.width - ((colCount - 1) * xSpacing + cardRect.x)) / 2;
	    	float yMargin = 10f;

    		Vector3 pos = new Vector3(
    			((i % colCount) * xSpacing) + (cardRect.x / 2) + xMargin,
    			-((cardRect.y + yMargin) * (i / colCount)) - (cardRect.y / 2) - yMargin,
    			0
    		);
    		contentHeight = -pos.y + cardRect.y;
    		cardObj.transform.localPosition = pos;
            this.cardObjs.Add(cardObj.GetComponent<Card>());
    	}

    	contentRect.sizeDelta = new Vector2(
    		contentRect.rect.width,
    		contentHeight
    	);
    }

    GameObject MakeCard(CardType cardType) {
    	RectTransform rect = this.scrollView.GetComponent<RectTransform>();
		GameObject cardObj = Creator.CreateCard(Vector3.zero, Vector3.zero, cardType, this.content.transform);
		Card card = cardObj.GetComponent<Card>();
		card.Mask(this.useUILayer ? WindowManager.UILayer : WindowManager.baseUILayer);
		card.SetCustomHoverAction(() => {
			bool isInView = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, Camera.main);
			if (!isInView) return;
			this.ShowThumbnail(cardObj, cardType);	
		}, this.RemoveThumbnail);
		return cardObj;
    }

    void RemoveThumbnail() {
    	Destroy(this.thumb);
    }

    void ShowThumbnail(GameObject cardObj, CardType cardType) {
    	GameObject thumb = Creator.CreateCard(Vector3.zero, Vector3.zero, cardType, this.transform);
    	Card card = thumb.GetComponent<Card>();
    	float scale = 41f;
		float cardWidth = card.cardSprite.GetComponent<SpriteRenderer>().sprite.bounds.size.x * scale;
		float scrollWidth = this.scrollView.GetComponent<RectTransform>().rect.width * this.scrollView.transform.localScale.x;
    	card.DisableInteraction();
        card.SetUICard(scale);
    	thumb.transform.localPosition = new Vector3(
    		this.scrollView.transform.localPosition.x + scrollWidth / 2 + cardWidth / 2 + 10f,
    		20f,
            0f
		);
		this.thumb = thumb;
    }

    private void ClearCards() {
    	foreach (Transform child in this.content.transform) {
    		Destroy(child.gameObject);
    	}
    }
}
