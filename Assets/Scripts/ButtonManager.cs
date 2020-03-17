using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
	public void NewGameClick() {
		Store.Reset();
		WindowManager.HideScreen(WindowManager.startPanel);
		WindowManager.ShowScreen(WindowManager.draftUI);
	}

	public void ExitBattleClick () {
		LevelGraph.instance.ReturnToMap();
	}

	public void EndTurnClick() {
		Battlefield.instance.EndPlayerTurn();
	}	

	public void DeckClick() {
		CardWindow.instance.PopulateDeck();
		CardWindow.instance.Toggle();
	}

	public void DiscardClick() {
		CardWindow.instance.PopulateDiscard();
		CardWindow.instance.Toggle();
	}

	public void LootCardsClick() {
		WindowManager.ClearOverlays();
		WindowManager.ShowScreen(WindowManager.lootDraft);
	}

	public void LeaveDraftClick(GameObject draft) {
		draft.GetComponent<Draft>().EndDraft();
	}

	public void ConfirmSelectionClick() {
		Hand.instance.OnConfirmSelection();	
	}
}
