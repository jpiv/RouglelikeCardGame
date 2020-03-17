using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour {
	public GameObject hpText;
	public GameObject shieldText;
	public GameObject shieldBar;
	public GameObject buffBar;
	public float hp = 100f;
	public float shield = 10f;
	public float maxHealth = 100f;

	private Vector3 fullHPPos;
	private Vector3 fullShieldPos;
	private LineRenderer line;
	private LineRenderer shieldLine;
	private List<Effect> buffs = new List<Effect>();
	private List<GameObject> buffObjects = new List<GameObject>();

	void Awake() {
		line = GetComponent<LineRenderer>();
		fullHPPos = line.GetPosition(1);
		shieldLine = this.shieldBar.GetComponent<LineRenderer>();
		fullShieldPos = shieldLine.GetPosition(1);
	}
	void Start() {
		this.UpdateShieldBar(this.shield);
	}

	public float BuffStartPos(float shield) {
		if (shield != 0) {
			float shieldPos = this.shieldText.transform.localPosition.x;
			Rect shieldRect = this.shieldText.GetComponent<RectTransform>().rect;
			return shieldPos + shieldRect.width;
		} else {
			float hpPos = this.hpText.transform.localPosition.x;
			Rect hpRect = this.hpText.GetComponent<RectTransform>().rect;
			return hpPos + hpRect.width;
		}
	}

	public void SetMaxHP(float maxHP) {
		float ratio = this.hp / this.maxHealth;
		this.maxHealth = maxHP;
		this.SetHP(maxHP * ratio);
	}	

	public void ForceUpdateHP(float newHp) {
		this.hp = newHp;
		this.UpdateHealthBar(newHp);
	}

	public void AddBuff(Effect effect) {
		this.buffs.Add(effect);
		List<Effect> buffsCopy = new List<Effect>(this.buffs);
		float newHP = this.hp;
		float newShield = this.shield;
		AnimationQueue.Add(() => this.RenderBuffs(buffsCopy, newShield), 0f);
	}

	public void RemoveBuff(int id){
		this.buffs.RemoveAll(effect => effect.id == id);
		List<Effect> buffsCopy = new List<Effect>(this.buffs);
		float newHP = this.hp;
		float newShield = this.shield;
		AnimationQueue.Add(() => this.RenderBuffs(buffsCopy, newShield), 0f);
	}

	public void SetHP(float hp) {
		this.hp = hp;
		float newShield = this.shield;
		AnimationQueue.Add(() => this.UpdateHealthBar(hp), 0f);
	}

	public void SetShield(float shield) {
		this.shield = shield;
		float newHp = this.hp;
		AnimationQueue.Add(() => this.UpdateShieldBar(shield), 0f);
	}

	private void UpdateHealthBar(float newHp) {
		float hpBarStartPos = this.line.GetPosition(0).x;
		float barEndPosition = hpBarStartPos + (newHp / this.maxHealth) * (fullHPPos.x - hpBarStartPos);
		this.line.SetPosition(1, new Vector3(barEndPosition, this.fullHPPos.y, this.fullHPPos.z));
		this.hpText.GetComponent<TextMeshPro>().text = Mathf.Ceil(newHp).ToString();
		HorizontalLayoutGroup hlg = this.buffBar.GetComponent<HorizontalLayoutGroup>();
	    LayoutRebuilder.ForceRebuildLayoutImmediate(this.buffBar.GetComponent<RectTransform>());
	}

	private void UpdateShieldBar(float newShield) {
		float shieldStartPos = this.shieldLine.GetPosition(0).x;
		float barEndPosition = shieldStartPos + (newShield / this.maxHealth) * (fullShieldPos.x - shieldStartPos);
		this.shieldLine.SetPosition(1, new Vector3(barEndPosition, this.fullShieldPos.y, this.fullShieldPos.z));
		if (newShield != 0) {
			this.shieldText.GetComponent<TextMeshPro>().text = Mathf.Ceil(newShield).ToString();
		} else {
			this.shieldText.GetComponent<TextMeshPro>().text = "";
		}
		List<Effect> buffsCopy = new List<Effect>(this.buffs);
		HorizontalLayoutGroup hlg = this.buffBar.GetComponent<HorizontalLayoutGroup>();
	    LayoutRebuilder.ForceRebuildLayoutImmediate(this.buffBar.GetComponent<RectTransform>());
		AnimationQueue.Add(() => this.RenderBuffs(buffsCopy, newShield), 0);
	}

	private void RenderBuffs(List<Effect> newBuffs, float newShield) {
		if (this == null) return;
		foreach (GameObject buffObj in this.buffObjects) {
			Destroy(buffObj);
		}
		this.buffObjects.Clear();
		float leftStart = this.BuffStartPos(newShield);
		for (int i = 0; i < newBuffs.Count; i++) {
			EffectType effect = newBuffs[i].effect;
			GameObject buffObject = Creator.CreateBuff(effect.image, Vector3.zero, this.buffBar.transform);
			this.buffObjects.Add(buffObject);
		}
	}

	public void RefreshHP() {
		this.SetHP(this.maxHealth);
	}
}
