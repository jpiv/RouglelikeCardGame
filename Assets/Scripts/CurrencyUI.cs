using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour {
	public static CurrencyUI instance;
	public TextMeshProUGUI bonesText;
	public TextMeshProUGUI soulsText;

	void Awake() {
		instance = this;
	}

	public static void UpdateBonesText(int bones) {
		CurrencyUI.instance._UpdateBonesText(bones);
	}

	public static void UpdateSoulsText(int souls) {
		CurrencyUI.instance._UpdateSoulsText(souls);
	}

	public void _UpdateBonesText(int bones) {
		this.bonesText.text = bones.ToString();
	}

	public void _UpdateSoulsText(int souls) {
		this.soulsText.text = souls.ToString();
	}
}
