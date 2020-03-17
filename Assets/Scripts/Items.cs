using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Items : MonoBehaviour {
    public static Items instance;
    public ItemType[] allItems;
    public ItemType armor;

	void Awake() {
    	instance = this;
    	TextAsset itemData = Resources.Load<TextAsset>("items");
    	allItems = JsonUtility.FromJson<ItemsType>(itemData.text).items;
        this.armor = allItems[0];
	}

}

[System.Serializable]
public struct ItemsType {
   public ItemType[] items;
}

[System.Serializable]
public struct ItemType {
    public string name;
    public string image;
    public StatType stats;
}

[System.Serializable]
public struct StatType {
    public int health;
    public int power;
}
