using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draft : InitialDraft {
	public bool isCardPack = false;

	void Awake() {
		this.scale = 24f;
	}

	public override void EndDraft() {
		if (this.isCardPack) {
			WindowManager.HideScreen(WindowManager.cardPack);
		} else {
			WindowManager.ClearOverlays();
			LevelGraph.instance.ReturnToMap();
		}
	}
}
