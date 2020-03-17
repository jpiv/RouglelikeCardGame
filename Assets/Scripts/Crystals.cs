using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crystals : MonoBehaviour {
	public static Crystals instance;
	public GameObject crystalsText;
	public int crystals = 4;

	private int maxCrystals = 4;

	void Awake() {
		instance = this;
		this.UpdateText();
	}

	public void AddMana(int mana) {
		this.crystals += mana;
		AnimationQueue.Add(this.UpdateText, 0);
	}

	public bool CanPlayCard(int cost) {
		return cost <= this.crystals;
	}

	public void Consume(int cost) {
		this.crystals -= cost;
		this.UpdateText();
	}

	public void Refresh() {
		this.crystals = this.maxCrystals;
		AnimationQueue.Add(this.UpdateText, 0);
	}

	private void UpdateText() {
		TextMeshPro textMesh = this.crystalsText.GetComponent<TextMeshPro>();
		textMesh.text = $"{ this.crystals.ToString() }/{ this.maxCrystals.ToString() }"; 
	}
}
