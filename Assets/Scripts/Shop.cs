using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
	public ShopItem shopItemPre;
	public GameObject cardPackContainer;
	public Sprite[] cardPackSprites;
	public Button BuyButton;

	private ShopItem selectedItem;

	void Start() {
		BuyButton.onClick.AddListener(this.OnBuy);
		this.DisableBuy();
		this.CreateCardPacks();
	}

	void OpenCardPack() {
		Draft cardPack = WindowManager.ShowScreen(WindowManager.cardPack).GetComponent<Draft>();
		float[] dropModifiers = LootTable.CreateForceDropModifiers(this.selectedItem.rarity);
		CardType includeCard = Deck.instance.RandomCard(dropModifiers);
		cardPack.SetIncludeCard(includeCard);
		cardPack.CreateDraft();
	}

	public void OnBuy() {
    	Store.AddBones(-this.selectedItem.cost);
    	this.OpenCardPack();
    	this.selectedItem.SetSelected(false);
    	this.selectedItem = null;
    	this.DisableBuy();
	}

	void SelectItem(ShopItem item) {
		if (Store.bones < item.cost) return;
		if (this.selectedItem == item) {
			item.SetSelected(false);
			this.DisableBuy();
			this.selectedItem = null;
			return;
		}
		if (this.selectedItem != null) {
			this.selectedItem.SetSelected(false);
		}
		this.selectedItem = item;
		item.SetSelected(true);
		this.EnableBuy();
	}

    void EnableBuy() {
    	this.BuyButton.interactable = true;
    }

    void DisableBuy() {
    	this.BuyButton.interactable = false;
    }

	void CreateCardPacks() {
		int rarity = LootTable.GetRarity();
		ShopItem item = this.AddShopItem(this.shopItemPre, this.cardPackContainer);
		item.SetImage(this.cardPackSprites[rarity]);
		item.SetOnClick(() => this.SelectItem(item));
		item.SetCost((rarity + 1) * 10);
		item.SetRarity(rarity);
	}

	ShopItem AddShopItem(ShopItem item, GameObject container) {
		GameObject gameObject = Instantiate(item.gameObject, container.transform);
		return gameObject.GetComponent<ShopItem>();
	}
}
