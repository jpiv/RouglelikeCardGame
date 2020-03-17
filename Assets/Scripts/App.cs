using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour {
	public static void ResetGame() {
		AnimationQueue.Clear();
		SceneHelper.CloseAll();
		Deck.instance.NewDeck();
		WindowManager.ClearOverlays();
		WindowManager.ShowScreen(WindowManager.startPanel);
	}

	public static void ClearChildren(Transform parent) {
		foreach (Transform child in parent) {
			Destroy(child.gameObject);
		}
	}
}
