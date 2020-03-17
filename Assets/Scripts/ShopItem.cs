using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
	public GameObject shopImage;
	public GameObject border;
	public Text text;
	public int cost;
	public int rarity;

	private Action _onClick;

	public void SetImage(Sprite image) {
		this.shopImage.GetComponent<Image>().sprite = image;
	}

	public void SetOnClick(Action onClick) {
		this._onClick = onClick;
	}

	public void SetCost(int cost) {
		this.cost = cost;
		this.text.text = $"x{ cost } Bones";
	}

	public void SetSelected(bool selected) {
		this.border.SetActive(selected);
	}

	public void SetRarity(int rarity) {
		this.rarity = rarity;
	}

	void OnMouseUpAsButton() {
		if (this._onClick == null) return;
		this._onClick();
	}
}
