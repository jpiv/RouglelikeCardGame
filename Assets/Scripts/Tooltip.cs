using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
	public GameObject tooltip;

	void Start() {
		tooltip.SetActive(false);
	}

	void OnMouseEnter() {
		tooltip.SetActive(true);
	}

	void OnMouseExit() {
		tooltip.SetActive(false);	
	}

	public void SetText(string text) {
		tooltip.GetComponentInChildren<Text>().text = text;
	}
}
