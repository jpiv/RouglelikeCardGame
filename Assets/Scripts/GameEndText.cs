using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndText : MonoBehaviour {
    public static void CreateText(GameObject gameEndAnimation, bool isVictory, Transform transform) {
		GameObject gameEndText = Instantiate(gameEndAnimation, transform);
		TextMeshPro textComponent = gameEndText.transform.Find("AnimationContainer/Text").GetComponent<TextMeshPro>();
		if (isVictory) {
			textComponent.text = "VICTORY!";
		} else {

			textComponent.text = "DEFEAT";
		}
    }
}
