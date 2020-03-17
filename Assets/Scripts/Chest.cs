using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
	private Animator animator;

    void Start() {
    	this.animator = GetComponent<Animator>(); 
    }

    void OnMouseUpAsButton() {
    	this.animator.SetBool("Open", true);
    }

    public void OpenChest() {
    	Draft cardPack = WindowManager.ShowScreen(WindowManager.cardPack).GetComponent<Draft>();	
        cardPack.isCardPack = false;
    }
}
