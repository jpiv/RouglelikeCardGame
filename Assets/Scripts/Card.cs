using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Rarity {
    public const int ORDINARY = 1;
    public const int EARTHLY = 2;
    public const int MYSTICAL = 3;
    public const int DIVINE = 4;
    public const int TRANSCENDENT = 5;
}

public class Card : MonoBehaviour {
    public bool isHovered = false;
    public bool isDragging = false;
    public bool canInteract = true; 
    public CardType cardType;
    public GameObject canvas;
    public GameObject descriptionBox;
    public GameObject titleBox;
    public GameObject manaBox;
    public GameObject hoverCardGlow;
    public GameObject selectedCardGlow;
    public GameObject cardSprite;
    public Sprite earthlyCardSprite;
    public Sprite mysticalCardSprite;
    public Sprite divineCardSprite;
    public Sprite transcendantCardSprite;
    public Vector3 defaultPos;
    public Vector3 defaultScale;
    public Quaternion defaultRot;

    private int index;
    private bool interactiveOnMouseUp = false;
    private float hoverYOffset = 2.75f;
    private float hoverScaleModifer = 1.5f;
    private GameObject cardImage;
    private Action hoverAction;
    private Action exitAction;
    private Action clickAction;

    void Start() {
        this.defaultPos = this.transform.localPosition;
        this.defaultRot = this.transform.localRotation;
        this.defaultScale = this.transform.localScale;
    }

    public void SetSelected(bool isSelected = true) {
        if (this == null) return;
        this.selectedCardGlow.SetActive(isSelected);
    }

    public void SetCard(CardType card) {
        this.cardType = card;
        this.RenderDetails();
        this.UpdateRarity();
    }

    private void UpdateRarity() {
        SpriteRenderer sr = this.cardSprite.GetComponent<SpriteRenderer>();
        switch (this.cardType.rarity) {
            case Rarity.EARTHLY:
                sr.sprite = this.earthlyCardSprite;
                break;
            case Rarity.MYSTICAL:
                sr.sprite = this.mysticalCardSprite;
                break;
            case Rarity.DIVINE:
                sr.sprite = this.divineCardSprite;
                break;
            case Rarity.TRANSCENDENT:
                sr.sprite = this.transcendantCardSprite;
                break;
            default:
                break;
        }
    }

    public void SetUICard(float scale) {
        this.DisableInteraction();
        this.transform.localScale = Vector3.one * scale;
        SpriteRenderer cardRenderer = this.cardSprite.GetComponent<SpriteRenderer>();
        cardRenderer.sortingLayerName = "UI";
        cardRenderer.sortingOrder = WindowManager.UILayer + 2;

        SpriteRenderer imageRenderer = this.cardImage.GetComponent<SpriteRenderer>();
        imageRenderer.sortingLayerName = "UI";
        imageRenderer.sortingOrder = WindowManager.UILayer + 1;

        Renderer glowRenderer = this.selectedCardGlow.GetComponent<Renderer>();
        glowRenderer.sortingLayerName = "UI";
        glowRenderer.sortingOrder = WindowManager.UILayer;

        this.canvas.GetComponent<Canvas>().overrideSorting = true;
        this.canvas.GetComponent<Canvas>().sortingLayerName = "UI";
        this.canvas.GetComponent<Canvas>().sortingOrder = WindowManager.UILayer + 3;
    }

    public void Mask(int layer) {
        this.SetUICard(1f);
        SpriteRenderer cardRenderer = this.cardSprite.GetComponent<SpriteRenderer>();
        cardRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        cardRenderer.sortingOrder = layer + 1;
        SpriteRenderer imageRenderer = this.cardImage.GetComponent<SpriteRenderer>();
        imageRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        cardRenderer.sortingOrder = layer;
        ParticleSystemRenderer glowRenderer = this.selectedCardGlow.GetComponent<ParticleSystemRenderer>();
        glowRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        this.canvas.GetComponent<Canvas>().overrideSorting = false;
    }

    public void DisableInteraction() {
        this.canInteract = false;
    }

    public void EnableInteraction() {
        this.canInteract = true;
    }

    private void RenderDetails() {
        this.RenderImage();
        this.RenderText();
    }

    private string MakeCardText(string text) {
        string finalText = text;
        foreach (EffectType effectType in this.cardType.effects) {
            if (effectType.modifiers.amount != 0) {
                string effectText = $"<color=blue>{ effectType.modifiers.amount.ToString() }</color>";
                finalText = finalText.Replace($"*{ effectType.name }", effectText);
            }
        }
        if (this.cardType.damage != 0) {
            float damage = this.cardType.damage + Store.GetPowerStat(); 
            string damageText = $"<color=yellow>{ damage.ToString() }</color>";
            finalText = finalText.Replace("*d", damageText);
        }
        return $"<color=black>{ finalText }</color>";
    }

    private void RenderText() {
        this.descriptionBox.GetComponent<TextMeshProUGUI>().text = this.MakeCardText(this.cardType.description);
        this.titleBox.GetComponent<TextMeshProUGUI>().text = this.cardType.name;
        this.manaBox.GetComponent<TextMeshProUGUI>().text = this.cardType.mana.ToString();
    }

    private void RenderImage() {
        GameObject cardImage = Resources.Load<GameObject>("CardImages/" + this.cardType.image);
        GameObject cardObject = Instantiate(cardImage, this.canvas.transform);
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        cardObject.transform.localPosition = new Vector3(-32.5f, 32.4f, -0.5f);
        cardObject.transform.localScale = new Vector3(59.99821f, 51.33867f, 1f);
        cardObject.transform.SetSiblingIndex(0);
        this.cardImage = cardObject;
    }

    public void SetIndex(int index) {
        this.index = index;
    }

    public void ZIndex(Card card, int index) {
        card.selectedCardGlow.GetComponent<Renderer>().sortingOrder = (index + 1) * WindowManager.UILayer + 1;
        card.hoverCardGlow.GetComponent<Renderer>().sortingOrder = (index + 1) * WindowManager.UILayer + 1;
        card.cardImage.GetComponent<SpriteRenderer>().sortingOrder = (index + 1) * WindowManager.UILayer + 2;
        this.cardSprite.GetComponent<SpriteRenderer>().sortingOrder = (index + 1) * WindowManager.UILayer + 3;
        card.canvas.GetComponent<Canvas>().sortingOrder = (index + 1) * WindowManager.UILayer + 3;
    }

    public void SetCustomHoverAction(Action hoverAction, Action exitAction) {
        this.hoverAction = hoverAction;
        this.exitAction = exitAction;
    }

    public void SetClickAction(Action clickAction) {
        this.clickAction = clickAction;
    }

    public void SetInteractiveOnMouseUp() {
        this.interactiveOnMouseUp = true;
    }

    void OnMouseUpAsButton() {
        if (this.clickAction != null) {
            this.clickAction();
        }
    }

    void OnMouseDrag() {
        this.isHovered = false;
        if (!this.canInteract) return;
        if (Battlefield.instance != null && Battlefield.instance.waitingForSelection) return;
        this.ResetTransform();
        Hand.isDragging = true;
        this.isDragging = true;
    	Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	this.transform.position = new Vector3(mousePos.x, mousePos.y, -15);
    	this.transform.localRotation = Quaternion.identity;
        this.ZIndex(this, 11);
    }

    void OnMouseUp() {
        if (this.interactiveOnMouseUp) {
            this.interactiveOnMouseUp = false;
            this.canInteract = true;
            Hand.isDragging = false;
            return;
        }
        if (!this.canInteract) return;
        Hand.instance.StartPlayCard(this.cardType, this.gameObject);
    }

    void OnMouseEnter() {
        if (Hand.isDragging) return;
        this.isHovered = true;
        if (this.hoverAction != null) {
            this.hoverAction();
            return;
        }
        this.hoverCardGlow.SetActive(true);
        this.transform.localPosition = new Vector3(
            this.transform.localPosition.x,
            this.hoverYOffset,
            -15
        );
        this.transform.localScale = this.transform.localScale * this.hoverScaleModifer;
        this.transform.localRotation = Quaternion.identity;
        this.ZIndex(this, 11);
    }

    void OnMouseExit() {
        this.isHovered = false;
        this.isDragging = false;
        this.hoverCardGlow.SetActive(false);
        if (Hand.isDragging) return;
        if (this.exitAction != null) {
            this.exitAction();
            return;
        }
        // this.ResetTransform();
    }

    public void ResetScale() {
        this.transform.localScale = this.defaultScale;
    }

    public void ResetZIndex() {
        this.ZIndex(this, this.index);
    }

    void ResetTransform() {
        this.transform.localPosition = this.defaultPos;
        this.transform.localRotation = this.defaultRot;
        this.transform.localScale = this.defaultScale;
        this.ResetZIndex();
    }
}

[System.Serializable]
public struct CardType {
    public int id;
	public string name;
	public string description;
	public string type;
    public string image;
    public string upgrade;
    public string targetType;
	public int mana;
	public float damage;
	public int rarity;
    public EffectType[] effects;
}